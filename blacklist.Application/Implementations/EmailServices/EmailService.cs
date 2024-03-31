 

 

namespace blacklist.Application.Implementations.EmailServices
{
    public class EmailService: IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpAccount _smtp;
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IFileSystemManagerService  _fileSystemManagerService;

		public EmailService(IFileSystemManagerService fileSystemManagerService, IHostEnvironment environment, IConfiguration config, SmtpAccount smtp, ILogger<EmailService> logger)
		{
			_fileSystemManagerService = fileSystemManagerService;
			_environment = environment;
			_config = config;
			_logger = logger;
            _smtp = smtp;
		}

		public bool SendEmail(MailMessage message)
        {
            try
            {
                string _mailsetting = _smtp.Settings;
                MailSettings _setting = JsonConvert.DeserializeObject<MailSettings>(_mailsetting);
                if (_setting.CopyAddresses.Count > 0)
                {
                    foreach (string address in _setting.CopyAddresses)
                    {
                        message.Bcc.Add(new MailAddress(address));
                    }
                }

                var smtpClient = new SmtpClient
                {
                    Host = _setting.Host,
                    Port = _setting.Port,
                    EnableSsl = _setting.EnableSsl,
                    DeliveryMethod = _setting.DeliveryMethod,
                    UseDefaultCredentials = _setting.UseDefaultCredentials,
                    Credentials = new System.Net.NetworkCredential(_setting.Credentials.Username, _setting.Credentials.Password)
                };
                message.From = new MailAddress(_smtp.Sender);
                smtpClient.UseDefaultCredentials = _setting.UseDefaultCredentials;
                smtpClient.Credentials = new System.Net.NetworkCredential(_setting.Credentials.Username, _setting.Credentials.Password);
                message.From = new MailAddress(_smtp.Sender);
                message.Subject = "Vaterba Support Application";
                message.IsBodyHtml = true;
                smtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "SendEmail inside EmailServiceIntegration for Recipient:" + message.To + ", Subject: " + message.Subject);
                return false;
            }
        }

   

        public bool SendUserCredentialsEmail(string userEmail, string password, UserDto userDto, string ActivationLink)
        {
            try
            {
                // Create the MailMessage, replace placeholders in the email content with user-specific information
                var message = new MailMessage();
                message.IsBodyHtml = true;

                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/WelcomeEmailTemplate.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                message.Body = htmlContent
                    .Replace("{USERNAME}", userDto.UserName)
                    .Replace("{EMAIL}", userEmail)
                    .Replace("{PASSWORD}", password)
                    .Replace("{ACTIVATIONLINK}", ActivationLink);
                message.To.Add(new MailAddress(userEmail));
                // Use your email service to send the message
                return SendEmail(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user credentials email.");
                return false;
            }
        }

        public bool SendPasswordResetOTP(string email, string name, string otp)
        {
            try
            {
                string _mailsetting = _smtp.Settings;
                MailSettings _setting = JsonConvert.DeserializeObject<MailSettings>(_mailsetting);
                var message = new MailMessage();



                message.IsBodyHtml = true;

                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/SendPasswordOTPTemplate.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                message.Body = htmlContent
                    .Replace("{USERNAME}", name)
                    .Replace("{OTP}", otp);
                message.To.Add(new MailAddress(email, name));

                return SendEmail(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EmailServiceIntegration => SendEmail() for Recipient: Waec Admin {email}");
                return false;
            }
        }

        public bool ResetPasswordEmail(string userEmail, string name)
        {
            try
            {
                var message = new MailMessage();
                message.IsBodyHtml = true;

                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/ResetPasswordEmailTemplate.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                message.Body = htmlContent
                    .Replace("{USERNAME}", name);
                message.To.Add(new MailAddress(userEmail));
                return SendEmail(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user credentials email.");
                return false;
            }
        }

        public bool ActivationEmail(string userEmail, string name)
        {
            try
            {
                var message = new MailMessage();
                message.IsBodyHtml = true;

                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/ActivationEmailTemplate.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                message.Body = htmlContent
                    .Replace("{USERNAME}", name);
                message.To.Add(new MailAddress(userEmail));
                return SendEmail(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user credentials email.");
                return false;
            }
        }

        public bool ForgetPasswordEmail(string email, string name, string resetLink)
        {
            try
            {
                string _mailsetting = _smtp.Settings;
                MailSettings _setting = JsonConvert.DeserializeObject<MailSettings>(_mailsetting);
                var message = new MailMessage();



                message.IsBodyHtml = true;

                string htmlPath = _environment.ContentRootPath + Path.DirectorySeparatorChar + "EmailTemplates/ForgetPasswordEmail.html";
                string htmlContent = Convert.ToString(Utilities.ReadHtmlFile(htmlPath));
                message.Body = htmlContent
                    .Replace("{USERNAME}", name)
                    .Replace("{RESETLINK}", resetLink);
                message.To.Add(new MailAddress(email, name));

                return SendEmail(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EmailServiceIntegration => SendEmail() for Recipient: Waec Admin {email}");
                return false;
            }
        }
    }
}
