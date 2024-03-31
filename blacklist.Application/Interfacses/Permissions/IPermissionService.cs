using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.Permissions
{
    public interface IPermissionService:ICRUD<PermissionDTO,Permission>
    {
    }
}
