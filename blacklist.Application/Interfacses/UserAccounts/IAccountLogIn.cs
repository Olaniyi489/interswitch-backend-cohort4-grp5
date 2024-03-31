namespace blacklist.Application.Interfacses.UserAccounts
{
    public interface IAccountLogIn
    {
        Task<ServerResponse<LogInResponse>> LogIn(string email, string password);
        Task<ServerResponse<bool>> CheckLoginCount(string language, string email);
    }
}