using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Domain.Entities;

namespace blacklist.Application.Common.DTOs
{
    public class OTPDto
    {
        public string OTP { get; set; }
        public string Email { get; set; }
        public OTPType OTPType { get; set; }
        public string NewPassword { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(OTP))
            {
                string message = $"OTP {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                string message = $" Email {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            if (OTPType <= 0)
            {
                string message = $" OTPType {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                string message = $"NewPassword {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            source = response;
            return true;
        }
    }
}
