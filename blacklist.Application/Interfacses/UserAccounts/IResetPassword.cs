using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.UserAccounts
{
    public interface IResetPassword
    {
        public Task<ServerResponse<string>> GeneratePasswordResetOTP(string email);
        public Task<ServerResponse<bool>> ResetPasswordWithOTP(OTPDto model);
        Task<ServerResponse<bool>> ChangePassword(string userId, ChangePasswordDTO model);
        //Task<ServerResponse<string>> ForgetPassword(string Email);
    }
}
