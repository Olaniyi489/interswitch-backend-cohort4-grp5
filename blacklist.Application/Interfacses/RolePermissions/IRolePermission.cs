using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.RolePermissions
{
    public interface IRolePermission
    {
        Task<ServerResponse<bool>> AssignPermissionsToRole(RolePermissionDTO request);
        Task<ServerResponse<bool>> UnassignPermissionsFromRole(RolePermissionDTO request);
        Task<ServerResponse<List<RolePermissionModel>>> GetAllRecord();
        Task<ServerResponse<List<RolePermissionModel>>> GetRecordById(string roleId);
        Task<ServerResponse<bool>> UpdateRolePermissions(RolePermissionUpdateDto request);
    }
}
