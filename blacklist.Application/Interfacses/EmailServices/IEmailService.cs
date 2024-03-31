using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.EmailServices
{
    public interface IEmailService
    {
        public bool SendEmail(MailMessage message);
        //public StringBuilder ReadHtmlFile(string htmlFileNameWithPath);
        public bool SendPasswordResetOTP(string email, string name, string otp);
        public bool SendUserCredentialsEmail(string userEmail, string password, UserDto userDto, string ActivationLink);
        public bool ForgetPasswordEmail(string email, string name, string resetLink);
        public bool ResetPasswordEmail(string userEmail, string name);
        public bool ActivationEmail(string userEmail, string name);
    }
}
