using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.Roles
{
    public interface IRoleService
    {
        public Task<ServerResponse<bool>> Create(RoleDTO request);
        public Task<ServerResponse<bool>> Delete(string id);
        public Task<ServerResponse<List<RoleModel>>> GetAllRecord();
        public Task<ServerResponse<RoleModel>> GetRecordById(string id);
        public Task<ServerResponse<bool>> Update(RoleDTO request);
    }
}
