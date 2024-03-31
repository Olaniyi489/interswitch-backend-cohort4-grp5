using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Application.Implementations;

namespace blacklist.Application.Common.DTOs
{
    public class RolePermissionDTO
    {
        public string RoleId { get; set; }
        public List<long> PermissionIds { get; set; }

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                string message = $"Role Id {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }

            if (PermissionIds == null || PermissionIds.Count == 0)
            {
                string message = $" Permission Id {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
                response.Message = message;
                response.Code = ResponseCodes.DATA_IS_REQUIRED;

                source = response;
                return false;
            }
            source = response;
            return true;
        }
    }

    //public class RolePermissionModel
    //{
    //    public string RoleId { get; set; }
    //    public List<long> PermissionIds { get; set; }
    //    public string PermissionName { get; set; }
    //}


    public class RolePermissionModel
    {
        public string RoleId { get; set; }
        public List<PermissionModels> Permissions { get; set; }
    }

    public class PermissionModels
    {
        public long PermissionId { get; set; }
        public string PermissionName { get; set; }
    }

    public class RolePermissionUpdateDto
    {
        public string RoleId { get; set; }
        public List<long>? RemovePermissionIds { get; set; } = new List<long>();
        public List<long>? AddPermissionIds { get; set; } 

        public bool IsValid(out ValidationResponse source, IMessageProvider messageProvider, string language)
        {
            var response = new ValidationResponse();
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                string message = $"Role Id {messageProvider.GetMessage(ResponseCodes.DATA_IS_REQUIRED, language)}";
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
