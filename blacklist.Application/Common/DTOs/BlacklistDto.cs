using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class BlacklistDto
    {
        public BlacklistCategory Category { get; set; }
        public string Reason { get; set; }
        public string? Email { get; set; }
        public DateTime BlacklistedAt { get; set; }
        public string? BlacklistedById { get; set; }
        public bool IsActive { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(Category.ToString()) || string.IsNullOrWhiteSpace(Email))
            {
                string message = $"{nameof(BlacklistCategory)} {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source= response;
                return false;
            }
            if (string.IsNullOrWhiteSpace(Reason))
            {
                string message = $" Reason {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            //if (string.IsNullOrWhiteSpace(BlacklistedById))
            //{
            //    string message = $" BlacklistedById {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
            //    response.Message = message;
            //    response.Code = ResponseCodes.DATA_IS_REQUIRED;

            //    source = response;
            //    return false;
            //}

            source = response;
            return true;
        }
      
    }

    public class BlackListUserDto
    {
        public string Email { get; set; }
        public string Reason { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
          
            if (string.IsNullOrWhiteSpace(Reason))
            {
                string message = $" Reason {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
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

            source = response;
            return true;
        }


    }

    public class BlacklistViewModel
    {
        public string Id { get; set; }  
        public string Category { get; set; }
        public string Reason { get; set; }
        public DateTime BlacklistedAt { get; set; }
        public string Email { get; set; }
        public string BlacklistedUserId { get; set; }
        public string BlacklistedById { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }

}
