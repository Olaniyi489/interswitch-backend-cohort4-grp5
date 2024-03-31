using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
      //  public string UserId { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                string message = $"CurrentPassword {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
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
            //if (string.IsNullOrWhiteSpace(UserId))
            //{
            //    string message = $"UserId {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
            //    response.Message = message;
            //    response.Code = ResponseCodes.DATA_IS_REQUIRED;

            //    source = response;
            //    return false;
            //}

            source = response;
            return true;
        }
    }
}
