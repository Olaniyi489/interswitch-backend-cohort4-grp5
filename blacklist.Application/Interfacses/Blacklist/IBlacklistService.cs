using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.Blacklist
{
    public interface IBlacklistService
    {
        Task<ServerResponse<bool>> CreateBlacklistByCategory(BlacklistDto request); 
        Task<ServerResponse<bool>> CreateBlacklistByEmail(BlackListUserDto request);
        Task<ServerResponse<Domain.Entities.Blacklist>> GetBlacklistedUserByEmail(string email);
        Task<ServerResponse<List<Domain.Entities.Blacklist>>> GetAllBlasklistedUsers();
        Task<ServerResponse<List<Domain.Entities.Blacklist>>> GetBlacklistedUserByCategory(string category);
        Task<ServerResponse<bool>> DeleteBlacklistUser(string email);
        Task<ServerResponse<bool>> DeleteBlacklistedUsers(BlacklistCategory category);
        Task<ServerResponse<bool>> UpdateBlacklist(BlacklistDto request);
    }
}
