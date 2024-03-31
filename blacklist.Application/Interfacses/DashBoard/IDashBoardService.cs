using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.DashBoard
{
    public interface IDashBoardService
    {
        Task<ServerResponse<DashBoardDTO>> GetDashDoardDetails(string roleId);
    }
}
