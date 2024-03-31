using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Domain.Entities
{
    [Table(nameof(RolePermission))]
    public class RolePermission : BaseObject
    {
        public string RoleId { get; set; }
        public long PermissionId { get; set; }
        public ApplicationRole Role { get; set; }
        public Permission Permission { get; set; }
    }
}
