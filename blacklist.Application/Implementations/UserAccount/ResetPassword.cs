

using Azure;
using Azure.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models;
using System;
using blacklist.Application.Common.DTOs;
using blacklist.Application.Common.Models;

namespace blacklist.Application.Implementations.UserAccount
{
    public class ResetPassword : ResponseBaseService, IResetPassword
    {
        private readonly IMessageProvider _messageProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAppDbContext _context;
       private readonly EmailHelper _emailHelper;
       private readonly EmailServiceBinding _emailServiceBinding;
        private readonly string _language;
        private readonly IDbContextTransaction _trans;
        private readonly ITokenService _tokenService;
        private readonly TokenConvert _tokenConvert;
        public readonly IHostEnvironment _hostEnvironment;  

        //private readonly EmailHelper _emailHelper;
        //private readonly EmailServiceBinding _emailServiceBinding;
        public ResetPassword(
            IMessageProvider messageProvider,
            IHttpContextAccessor httpContext,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ITokenService tokenService,
            IAppDbContext context,
            EmailServiceBinding emailServiceBinding,
            EmailHelper emailHelper,
            IHostEnvironment hostEnvironment) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService;
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _context = context;
            _trans = _context.Begin();
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _tokenService = tokenService;
            _tokenConvert = _tokenService.GetToken();
            _emailServiceBinding = emailServiceBinding;
            _emailHelper = emailHelper;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ServerResponse<bool>> ChangePassword(string userId, ChangePasswordDTO model)
        {          
           var response = new ServerResponse<bool>();

            if (!model.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                response.IsSuccessful = false;
                response.Data = false;
                SetErrorValidation(response, source.Code, source.Message);
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                response.IsSuccessful = false;
                response.Data = false;
                SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
                return response;
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
            }

            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);

            if (!passwordCheckResult)
            {
                response.IsSuccessful = false;
                response.Data = false;
                SetError(response, ResponseCodes.INVALID_PASSWORD, _language);
                return response;
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                // Handle change password failure
                response.IsSuccessful = false;
                response.Data = false;
                // Set error message accordingly
                return response;
            }

            // If change password succeeded, send email notification
            string htmlPath = _hostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/ActivationEmailTemplate.html";
            string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
            var body = htmlContent.Replace("{USERNAME}", user.FullName);

            var emailPayLoad = new EmailServiceModel
            {
                from = _emailServiceBinding?.Sender ?? "",
                messageBody = body,
                projectCode = _emailServiceBinding?.ProjectCode ?? "",
                to = user.Email,
                sentNow = true,
                subject = "User Registration",
                recieverName = user.FullName,
                scheduleDate = DateTime.Now,
                senderName = _emailServiceBinding?.SenderName ?? "N/A"
            };

            

            if (user.EmailConfirmed || !user.EmailConfirmed)
            {
                // Email sent successfully, commit transaction
                await _context.SaveChangesAsync();
                await _trans.CommitAsync();

                var emailSuccess = await _emailHelper.SendMail(emailPayLoad);
                if (emailSuccess.statusCode == "200")
                {
                    SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
                }
                
            }
            else
            {
                _trans.Rollback();
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            return response;
           
        }
    

        public async Task<ServerResponse<string>> GeneratePasswordResetOTP(string email)
        {
            var response = new ServerResponse<string>();
            if (email == null)
            {
                response.IsSuccessful = false;
                response.Data = null;
                SetError(response, ResponseCodes.DATA_IS_REQUIRED, _language);

                return response;
            }
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                response.IsSuccessful = false;
                response.Data = null;
                SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);

                return response;
            }
             
            var otp = GenerateRandomOTP();

                var otpRecord = new OTPs
                {
                    OTP = Convert.ToString(otp),
                    Email = user.Email,
                    OTPType = "ResetPassword"
                };

                _context.OTPs.Add(otpRecord);

            await _context.SaveChangesAsync();

            string htmlPath = _hostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/SendPasswordOTPTemplate.html";
            string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
            var body = htmlContent.Replace("{USERNAME}", user.FullName).Replace("{OTP}", otp);
            var emailPayLoad = new EmailServiceModel
            {
                from = _emailServiceBinding?.Sender ?? "",
                messageBody = body, //$"Kindly find your password :{Password} and user name {request.Email} Link {ActivationLink}",
                projectCode = _emailServiceBinding?.ProjectCode ?? "",
                to = user.Email,
                sentNow = true,
                subject = "Reset Password",
                recieverName = user.FirstName,
                scheduleDate = DateTime.Now,
                senderName = _emailServiceBinding?.SenderName ?? "N/A",



            };
            // var emailSuccess = _emailService.SendPasswordResetOTP(user.Email, user.FirstName, otp);

            var emailSuccess = await _emailHelper.SendMail(emailPayLoad);

            if (emailSuccess.statusCode=="200")
            {
                await _trans.CommitAsync();
                response.IsSuccessful = true;
                response.Data = null;
                SetSuccess(response, "Successful" , ResponseCodes.SUCCESS, _language);
            }
            else
            {
                response.IsSuccessful = false;
                response.Data = null;
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            return response;
        }

        public async Task<ServerResponse<bool>> ResetPasswordWithOTP(OTPDto model)
        {
            var response = new ServerResponse<bool>();

            if (!model.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                response.IsSuccessful = false;
                response.Data = false;
                SetErrorValidation(response, source.Code, source.Message);

                return response;
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                response.IsSuccessful = false;
                response.Data = false;
                SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);

                return response;
            }

            var otpRecord = await _context.OTPs.FirstOrDefaultAsync(o => o.Email == model.Email && o.OTP == model.OTP);

            if (otpRecord == null)
            {
                response.IsSuccessful = false;
                response.Data = false;
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_OBJECT_MAPPING,
                    ResponseDescription = "Invalid or expired OTP"
                };
                return response;
            }

            if (otpRecord.OTPType == "ResetPassword")
            {

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (resetPasswordResult.Succeeded)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    response.IsSuccessful = false;
                    response.Data = false;
                    response.Error = new ErrorResponse
                    {
                        ResponseCode = ResponseCodes.INVALID_OBJECT_MAPPING,
                        ResponseDescription = "Wrong OTP Type"
                    };

                    return response;
                }

                _context.OTPs.Remove(otpRecord);
                await _context.SaveChangesAsync();
            }
            else
            {
                response.IsSuccessful = false;
                response.Data = false;
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_OBJECT_MAPPING,
                    ResponseDescription = "Invalid OTP type"
                };
            }

            var emailSuccess = _emailService.ResetPasswordEmail(user.Email, user.FirstName);

            if (emailSuccess)
            {
                await _trans.CommitAsync();
                response.IsSuccessful = true;
                response.Data = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            }
            else
            {
                response.IsSuccessful = false;
                response.Data = false;
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            return response;
        }


        //public async Task<ServerResponse<string>> ForgetPassword(string Email)
        //{
        //    var response = new ServerResponse<string>();

        //    if (Email == null)
        //    {
        //        response.IsSuccessful = false;
        //        response.Data = null;
        //        SetError(response, ResponseCodes.DATA_IS_REQUIRED, _language);

        //        return response;
        //    }

        //    var user = await _userManager.FindByEmailAsync(Email);

        //    if (user == null)
        //    {
        //        response.IsSuccessful = false;
        //        response.Data = null;
        //        SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);

        //        return response;
        //    }

        //    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    var resetLink = GenerateResetLink(user.Id, resetToken);

        //    var emailSuccess = _emailService.ForgetPasswordEmail(user.Email, user.UserName, resetLink);

        //    if (emailSuccess)
        //    {
        //        response.IsSuccessful = true;
        //        response.Data = null;
        //        SetSuccess<string>(response, "null", ResponseCodes.SUCCESS, _language);
        //    }
        //    else
        //    {
        //        response.IsSuccessful = false;
        //        response.Data = null;
        //        SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);

        //        return response;
        //    }

        //    return response;
        //}

        private string GenerateResetLink(string userId, string resetToken)
        {
            var baseUrl = GetBaseUrl();

            var resetLink = baseUrl + "/account/change-password?userId=" + userId + "&token=" + resetToken;

            return resetLink;
        }

        private string GetBaseUrl()
        {
            var request = _httpContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return baseUrl;
        }


        private string GenerateRandomOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString("D6");
        }

    }
}
