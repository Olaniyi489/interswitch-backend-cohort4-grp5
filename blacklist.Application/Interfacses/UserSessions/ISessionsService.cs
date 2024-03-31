using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.UserSessions
{
    public interface ISessionsService
    {
        Task<ServerResponse<bool>> CreateSessionAsync(SessionDTO request, string language);
        Task<ServerResponse<bool>> DeleteSessionAsync(string userId, string language);
        Task<ServerResponse<bool>> UpdateSessionAsync(UpdateSessionDTO request, string language);
    }
}
