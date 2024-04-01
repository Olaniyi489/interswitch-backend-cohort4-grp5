
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph.Models;
using blacklist.Application.Common.Models;
using blacklist.Application.Implementations.EmailServices;
using blacklist.Application.Interfacses;
using blacklist.Domain.Entities;

namespace blacklist.Application.Implementations.UserAccounts
{
    public class UserService : ResponseBaseService, IUserService

    {
        private readonly ILogger<UserService> _logger;
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IAppDbContext _context;
        private readonly IDbContextTransaction _trans;
        private readonly string _language;
        private readonly IEmailService _emailService;
        private readonly IHostEnvironment _environment;
        private readonly EmailHelper _emailHelper;
        private readonly EmailServiceBinding _emailServiceBinding;
        private readonly ITokenService _tokenService;
        public UserService(IMessageProvider messageProvider,
            IHttpContextAccessor httpContext,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IEmailService emailService,
            IAppDbContext context,
            IHostEnvironment hostEnvironment,
            RoleManager<ApplicationRole> roleManager,
            EmailHelper emailHelper,
            EmailServiceBinding emailServiceBinding,
            ITokenService tokenService, ILoggerFactory log) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _config = config ?? throw new ArgumentNullException(nameof(config)); ;
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _trans = _context.Begin();
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _emailService = emailService ?? throw new ArgumentNullException("emailService");
            _environment = hostEnvironment ?? throw new ArgumentNullException("hostEnvironment");
            _emailHelper = emailHelper ?? throw new ArgumentNullException("emailHelper");
            _emailServiceBinding = emailServiceBinding ?? throw new ArgumentNullException("emailServiceBinding");
            _tokenService = tokenService ?? throw new ArgumentNullException("tokenService");
            _logger = log.CreateLogger<UserService>();
        }


        public async Task<ServerResponse<UserDto>> CreateUserAsync(RegisterViewModel request)
        {
            var response = new ServerResponse<UserDto>();

              _logger.LogInformation($"Inside CreateUserAsync {JsonConvert.SerializeObject(request)}");
            if (!request.IsValid(out ValidationResponse error, _messageProvider, _httpContext))
            {
                return SetError(response, error.Code, error.Message, _language);
            }
            var token = _tokenService.GetToken();
            if (token is null || token.User is null)
            {
                return SetError(response, ResponseCodes.INVALID_USER, _language);
            }


            //if (!request.Email.EndsWith("@vatebra.com"))
            //{
            //    return SetError(response, ResponseCodes.INVALID_OBJECT_MAPPING, _language);
            //}

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                SetError(response, ResponseCodes.RECORD_EXISTS, _language);
                return response;
            }


            UserDto userDto = null;
            // Create a new user
            var newUser = request.Adapt<ApplicationUser>();
            newUser.UserName = request.Email;
            newUser.Id = Guid.NewGuid().ToString();
            newUser.DateCreated = DateTime.Now;
            newUser.Occupation = "Organization";
            newUser.Department = request.Department.ToString();
            newUser.CountryId = 1;
            newUser.IsActive = true;
            newUser.IsBlacklisted = false;
            newUser.EmailConfirmed = false;
            newUser.FullName = $"{request.FirstName} {request.LastName}";
            var Password = PasswordGenHelper.GenerateRandomPassword(8);
            var passwordHash = _userManager.PasswordHasher.HashPassword(newUser, Password);

            newUser.PasswordHash = passwordHash;
            bool saved = false;
            var result = await _userManager.CreateAsync(newUser, Password);
            _logger.LogInformation($"User Has been Created {JsonConvert.SerializeObject(newUser)}, {JsonConvert.SerializeObject(result)}");
            if (result.Succeeded)
            {
                int count = request.Role.Count();

                // Assign the selected role to the user
                foreach (var role in request.Role)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(role.RoleName);
                    if (roleExists)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(newUser, role.RoleName);

                        if (roleResult.Succeeded)
                        {
                            count--;

                            saved = true;

                        }
                        else
                        {
                            if (count == request.Role.Count())
                            {
                                await _trans.RollbackAsync();
                                SetError(response, ResponseCodes.INVALID_ROLE, _language);
                                return response;
                            }

                            //
                        }
                    }

                }
            }
            else
            {
                _logger.LogInformation($"Could not create User {newUser}");
                await _trans.RollbackAsync();
                response.Error = new ErrorResponse
                {
                    ResponseDescription = result.Errors.FirstOrDefault()?.Description,
                    ResponseCode = ResponseCodes.EXCEPTION
                };
                //SetError(response, ResponseCodes.DUPPLICATE_USERNAME, _language);
            }
            if (saved)
            {
                userDto = newUser.Adapt<UserDto>();
                var ActivationLink = GenerateActivationLink(newUser.Id);
                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/WelcomeEmailTemplate.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                var body = htmlContent
                    .Replace("{USERNAME}", userDto.UserName)
                    .Replace("{EMAIL}", request.Email)
                    .Replace("{PASSWORD}", Password)
                    .Replace("{ACTIVATIONLINK}", ActivationLink);
                var emailPayLoad = new EmailServiceModel
                {
                    from = _emailServiceBinding?.Sender ?? "",
                    messageBody = body, //$"Kindly find your password :{Password} and user name {request.Email} Link {ActivationLink}",
                    projectCode = _emailServiceBinding?.ProjectCode ?? "",
                    to = request.Email,
                    sentNow = true,
                    subject = "User Registration",
                    recieverName = newUser.FullName,
                    scheduleDate = DateTime.Now,
                    senderName = _emailServiceBinding?.SenderName ?? "N/A",


                };
              var emailSuccess = await _emailHelper.SendMail(emailPayLoad);
               //  var emailSuccess = _emailService.ActivationEmail(user.Email, user.FirstName);

                if (emailSuccess.statusCode == "200")
                {
                    await _trans.CommitAsync();

                    SetSuccess(response, userDto, ResponseCodes.SUCCESS, _language);
                }
                else
                {

                    SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
                }
            }
            else
            {
                userDto = newUser.Adapt<UserDto>();
                var ActivationLink = GenerateActivationLink(newUser.Id);
                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/WelcomeEmailTemplate.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                var body = htmlContent
                    .Replace("{USERNAME}", userDto.UserName)
                    .Replace("{EMAIL}", request.Email)
                    .Replace("{PASSWORD}", Password)
                    .Replace("{ACTIVATIONLINK}", ActivationLink);
                var emailPayLoad = new EmailServiceModel
                {
                    from = _emailServiceBinding?.Sender ?? "",
                    messageBody = body, //$"Kindly find your password :{Password} and user name {request.Email} Link {ActivationLink}",
                    projectCode = _emailServiceBinding?.ProjectCode ?? "",
                    to = request.Email,
                    sentNow = true,
                    subject = "User Registration",
                    recieverName = newUser.FullName,
                    scheduleDate = DateTime.Now,
                    senderName = _emailServiceBinding?.SenderName ?? "N/A",


                };
                var emailSuccess = await _emailHelper.SendMail(emailPayLoad);
                //  var emailSuccess = _emailService.ActivationEmail(user.Email, user.FirstName);

                //if (emailSuccess.statusCode == "200")
                //{
                    await _trans.CommitAsync();

                    SetSuccess(response, userDto, ResponseCodes.SUCCESS, _language);
               // }
               
            }

            return response;

        }
        public async Task<ServerResponse<bool>> ActivateAccount(string id)
        {
            var response = new ServerResponse<bool>();
            if (string.IsNullOrEmpty(id))
            {

                SetError(response, ResponseCodes.DATA_IS_REQUIRED, _language);

                return response;
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {

                SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);

                return response;
            }

            user.EmailConfirmed = true;
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            await _context.SaveChangesAsync();


            string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/ActivationEmailTemplate.html";
            string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
            var body = htmlContent
                    .Replace("{USERNAME}", user.FullName);


            var emailPayLoad = new EmailServiceModel
            {
                from = _emailServiceBinding?.Sender ?? "",
                messageBody = body, //$"Kindly find your password :{Password} and user name {request.Email} Link {ActivationLink}",
                projectCode = _emailServiceBinding?.ProjectCode ?? "",
                to = user.Email,
                sentNow = true,
                subject = "User Registration",
                recieverName = user.FullName,
                scheduleDate = DateTime.Now,
                senderName = _emailServiceBinding?.SenderName ?? "N/A",

                //otherEmails = new OtherEmail[]
                // {
                //      new OtherEmail{  ccRecieverEmail=token?.User.Email, bbcRecieverEmail= token?.User.Email,  bbcRecieverName=token?.User.FirstName, ccRecieverName=token?.User.FirstName}
                // }

            };

            var emailSuccess = await _emailHelper.SendMail(emailPayLoad);
            // var emailSuccess = _emailService.ActivationEmail(user.Email, user.FirstName);

            if (emailSuccess.statusCode == "200")
            {
                await _trans.CommitAsync();

                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

            }
            else
            {

                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            return response;
        }
        public async Task<ServerResponse<bool>> DeleteUserProfile(string email)
        {
            var response = new ServerResponse<bool>();
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var success = await _userManager.DeleteAsync(user);

                if (success.Succeeded)
                {
                    await _trans.CommitAsync();
                    response.IsSuccessful = true;
                    SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

                }
                else
                {
                    await _trans.RollbackAsync();
                    SetError(response, ResponseCodes.EXCEPTION, _language);


                }
            }
            else
            {
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            return response;
        }

        public async Task<ServerResponse<bool>> DisableUserProfile(string email)
        {
            var response = new ServerResponse<bool>();

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                user.IsActive = false;

                await _userManager.UpdateAsync(user);

                await _trans.CommitAsync();

                response.IsSuccessful = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            }
            else
            {
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }


            return response;
        }
        public async Task<ServerResponse<bool>> ActivateUserProfile(string email)
        {
            var response = new ServerResponse<bool>();


            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                user.IsActive = true;

                await _userManager.UpdateAsync(user);

                await _trans.CommitAsync();

                response.IsSuccessful = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            }
            else
            {
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }


            return response;
        }
        public async Task<ServerResponse<UserDto>> GetProfile(string userId)
        {
            var response = new ServerResponse<UserDto>();

            try
            {
                var userWithRoles = await _context.Users
                    .Where(user => user.Id == userId)
                    .Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new { user, userRole })
                    .Join(_context.Roles, ur => ur.userRole.RoleId, role => role.Id, (ur, role) => new { ur.user, ur.userRole.RoleId, role.Name })
                    .GroupBy(x => x.user.Id)
                    .Select(group => new UserDto
                    {
                        UserName = group.First().user.UserName,
                        Email = group.First().user.Email,
                        PhoneNumber = group.First().user.PhoneNumber,
                        Id = group.First().user.Id,
                        FirstName = group.First().user.FirstName,
                        LastName = group.First().user.LastName,
                        IsActive = group.First().user.IsActive,    
                        DateCreated = group.First().user.DateCreated,
                        UserRoles = group.Select(x => new RoleDto
                        {
                            RoleId = x.RoleId,
                            RoleName = x.Name
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (userWithRoles != null)
                {
                    response.IsSuccessful = true;
                    response.Data = userWithRoles;
                    response.SuccessMessage = "user details successfully retrieved";
                }
                else
                {
                    response.IsSuccessful = false;
                    response.SuccessMessage = "User not found.";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or set response error information
                response.IsSuccessful = false;
                response.SuccessMessage = "An error occurred while fetching the user.";
                // LogError(ex.Message);
            }

            return response;
        }
        public async Task<ServerResponse<List<UserDto>>> GetAllUsers()
        {
            var response = new ServerResponse<List<UserDto>>();

            try
            {

                // var users=await _context.UserRoles.Include(p=>p.us)
                var usersWithRoles = await _context.Users
                    .Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new { user, userRole })
                    .Join(_context.Roles, ur => ur.userRole.RoleId, role => role.Id, (ur, role) => new { ur.user, ur.userRole.RoleId, role.Name })
                    .GroupBy(x => x.user.Id)
                    .Select(group => new UserDto
                    {
                        UserName = group.First().user.UserName,
                        Email = group.First().user.Email,
                        PhoneNumber = group.First().user.PhoneNumber,
                        Id = group.First().user.Id,
                        FirstName = group.First().user.FirstName,
                        LastName = group.First().user.LastName,
                        IsActive = group.First().user.IsActive,
                        DateCreated = group.First().user.DateCreated,
                        UserRoles = group.Select(x => new RoleDto
                        {
                            RoleId = x.RoleId,
                            RoleName = x.Name
                        }).ToList()
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = usersWithRoles;
                response.SuccessMessage = "User's details successfully received.";

            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or set response error information
                response.IsSuccessful = false;
                response.SuccessMessage = "An error occurred while fetching users.";
                // LogError(ex.Message);
            }

            return response;
        }


        //public async Task<ServerResponse<bool>> IsUserExists(string userEmail)
        //{

        //    var response = new ServerResponse<UserDto>();
        //    var data = await _context.GetData<User>("Exec [dbo].[SP_GetUsers]\", new SqlParameter(\"@id\", userId)");
        //    response.IsSuccessful = true;
        //    response.Data = data.AsQueryable().ProjectToType<UserDto>().FirstOrDefault();
        //    return response;
        //}

        public async Task<ServerResponse<bool>> IsUserExists(string userEmail)
        {

            var response = new ServerResponse<bool>();
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null && user.IsActive)
            {
                response.IsSuccessful = true;
                response.SuccessMessage = "User exist";
            }
            else
            {
                response.IsSuccessful = false;
                response.SuccessMessage = "User does not exists";
            }

            return response;
        }

        public async Task<ServerResponse<bool>> UpdateProfile(UpdateUser request)
        {
            _logger.LogInformation($"Inside UpdateProfile {JsonConvert.SerializeObject(request)}");

            var response = new ServerResponse<bool>();

            //if(!request.IsValid(out ValidationResponse source, _messageProvider, _httpContext))
            //{
            //    return SetError(response, source.Code,source.Message, _language);
            //}

            if (!request.Email.EndsWith("@vatebra.com"))
            {
                return SetError(response, ResponseCodes.INVALID_OBJECT_MAPPING, _language);
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
            {

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;

                var updateResult = await _userManager.UpdateAsync(user);
                var removeRole = new List<string>();
                List<string> nullOrEmptyRole = null;
                if (updateResult.Succeeded)
                {

                    if (request.RoleNames != null && request.RoleNames.Count > 0)
                    {
                        var existingRoles = await _userManager.GetRolesAsync(user);

                        var roles = request.RoleNames.Where(p => p != "" && p.ToLower() != "string").ToList();

                        if (roles != null && roles.Count > 0)
                        {

                            nullOrEmptyRole = request.RoleNames.Except(roles)?.ToList();
                            var rolesToRemove = existingRoles.Except(roles)?.ToList();

                            var rolesToAdd = (roles).Except(existingRoles).ToList();

                            if (rolesToRemove.Any())
                            {
                                // _logger.LogInformation($"^^^^^^^ rolesToRemove {JsonConvert.SerializeObject(rolesToRemove)}");
                                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                            }

                            if (rolesToAdd.Any())
                            {
                                // _logger.LogInformation($" ***rolesToAdd {JsonConvert.SerializeObject(rolesToAdd)}");
                                await _userManager.AddToRolesAsync(user, rolesToAdd);
                            }
                        }


                    }

                    await _trans.CommitAsync();
                    if (request.RoleNames == null)
                    {
                        SetSuccess(response, true, ResponseCodes.INVALID_ROLE, _language);
                    }
                    else if (nullOrEmptyRole != null)
                    {
                        SetSuccess(response, true, ResponseCodes.EMPTY_ROLES, _language);
                    }
                    else
                    {
                        SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
                    }

                }
                else
                {
                    await _trans.RollbackAsync();
                    SetError(response, ResponseCodes.EXCEPTION, _language);
                }
            }
            else
            {
                SetError(response, ResponseCodes.INVALID_USER, _language);
            }

            return response;
        }

        public async Task<ServerResponse<List<UserDto>>> GetAllUserByRole(string roleName)
        {
            var response = new ServerResponse<List<UserDto>>();

            try
            {
                var usersWithRoles = await _context.Users
                    .Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userRole) => new { user, userRole })
                    .Join(_context.Roles, ur => ur.userRole.RoleId, role => role.Id, (ur, role) => new { ur.user, ur.userRole.RoleId, role.Name })
                    .Where(x => x.Name == roleName) // Filter by the specified role name
                    .GroupBy(x => x.user.Id)
                    .Select(y => new UserDto
                    {
                        UserName = y.First().user.UserName,
                        Email = y.First().user.Email,
                        PhoneNumber = y.First().user.PhoneNumber,
                        Id = y.First().user.Id,
                        FirstName = y.First().user.FirstName,
                        LastName = y.First().user.LastName,
                        IsActive = y.First().user.IsActive,
                        UserRoles = y.Select(x => new RoleDto
                        {
                            RoleId = x.RoleId,
                            RoleName = x.Name
                        }).ToList()
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = usersWithRoles;
                response.SuccessMessage = $"Users with role '{roleName}' successfully retrieved.";
            }
            catch (Exception ex)
            {
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            return response;
        }

        private string GenerateActivationLink(string userId)
        {
            //var baseUri = "https://support-application-system.vercel.app/account/?id=dadfasdf";

            //var activationLink = $"{baseUri}/change-password?userId={userId}";

            //return activationLink;
            var baseUri = "https://support-application-system.vercel.app";
            var activationLink = $"{baseUri}/change-password/{userId}";
            return activationLink;
        }
        private string GetBaseUrl()
        {
            var request = _httpContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return baseUrl;
        }

        public async Task<ServerResponse<List<UserDtoV2>>> SearchUserByName(string name)
        {
            var response = new ServerResponse<List<UserDtoV2>>();

            var user = await _context.Users.Where(p => p.LastName.ToLower() == name.ToLower() || p.FirstName.ToLower() == name.ToLower())
                .Select(p => new UserDtoV2 { Name = p.FullName, Id = p.Id }).ToListAsync();
            response.IsSuccessful = true;
            response.Data = user;
            return response;
        }

       // public async Task<ServerResponse<bool>>UpdateUserRole()
    }
}
