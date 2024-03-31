using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.UserAccounts
{
    public interface IAccountLogout
    {
        Task<ServerResponse<bool>> LogOut();
        Task ClearsSession();
      
    }
}
