namespace blacklist.Application.Interfacses.UserAccounts
{
    public interface IUserService
    {

        Task<ServerResponse<bool>> IsUserExists(string userEmail);
        Task<ServerResponse<bool>> DeleteUserProfile(string email);
        Task<ServerResponse<bool>> DisableUserProfile(string email);
        Task<ServerResponse<UserDto>> GetProfile(string userId);
        Task<ServerResponse<bool>> UpdateProfile(UpdateUser request);
        Task<ServerResponse<UserDto>> CreateUserAsync(RegisterViewModel model);
        Task<ServerResponse<bool>> ActivateAccount(string id);
        Task<ServerResponse<List<UserDto>>> GetAllUsers();
        Task<ServerResponse<List<UserDto>>> GetAllUserByRole(string roleName);
        Task<ServerResponse<bool>> ActivateUserProfile(string email);
        public Task<ServerResponse<List<UserDtoV2>>> SearchUserByName(string name);
       
    }
}

