using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class PermissionDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(Name))
            {
                string message = $"Name {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                string message = $" Description {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            source = response;
            return true;
        }



    }

    public class PermissionModel:BaseObjectModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }

}
