using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.DTOs
{
    public class LogInResponse
    {
        public UserDto User { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public Tokens Token { get; set; }
        public List<PermissionDTO> Permissions { get; set; }

    }
 
}
