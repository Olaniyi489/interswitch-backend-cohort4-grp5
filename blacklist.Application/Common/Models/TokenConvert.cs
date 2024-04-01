using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Common.Models
{
    public class TokenConvert
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public List<GetPermissionModel> UserPermissions { get; set; }
        public UserDto User { get; set; }

        
    }

    
}
