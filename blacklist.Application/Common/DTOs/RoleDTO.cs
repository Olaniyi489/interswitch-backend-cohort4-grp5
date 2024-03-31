using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class RoleDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string RoleDescription { get; set; }
        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(Name))
            {
                string message = $"Role name {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            if (string.IsNullOrWhiteSpace(Department))
            {
                string message = $"Department {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            if (string.IsNullOrWhiteSpace(RoleDescription))
            {
                string message = $"Role description {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            source = response;
            return true;
        }
   
    }
    public class RoleModel:BaseObjectModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Department { get; set; }
        public DateTime DateCreated { get; set; }
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
   
    }
}
