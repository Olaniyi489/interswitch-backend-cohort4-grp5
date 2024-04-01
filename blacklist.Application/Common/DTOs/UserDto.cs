using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public partial class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlacklisted { get; set; }
        public DateTime DateCreated { get; set; }
        public List<RoleDto> UserRoles { get; set; } = new List<RoleDto>();

    }
 public partial class UserDtoV2
    {
        public string Name { get; set; }
      
        public string Id { get; set; }
        

    }
    public class RoleDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public partial class UpdateUser
	{	 
		public string Email { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
        public List<string> RoleNames { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, IHttpContextAccessor httpContext)
        {
            var lang = Convert.ToString(httpContext?.HttpContext?.Request?.Headers[ResponseCodes.LANGUAGE]);
            var response = new ValidationResponse();
            if (string.IsNullOrEmpty(Email))
            {
                var message = $"Email {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                var message = $"Firtname {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                var message = $"Lastname {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }
            if (RoleNames == null || RoleNames.Count <= 0)
            {
                var message = $"Role {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, lang)}";
                response.Code = ResponseCodes.DATA_IS_REQUIRED;
                response.Message = message;
                source = response;
                return false;
            }



            source = response;
            return true;
        }
    }
    public partial class AzureUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Code { get; set; }
        public string AuthCode { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string Occupation { get; set; }
        public string Role { get; set; }

    }
}
