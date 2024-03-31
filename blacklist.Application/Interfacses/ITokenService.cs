


using System.Security.Claims;

namespace blacklist.Application.Interfacses
{
    public interface ITokenService
    {
 

		Task<Tokens> GenerateToken(UserDto user);
		Task<Tokens> GenerateRefreshToken(UserDto user);
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
		TokenConvert GetToken(string tok = "");

        Task<bool> AddUserRefreshTokens(UserRefreshTokenDto request);
		Task DeleteUserRefreshTokens(string username, string refreshToken);
		Task<UserRefreshTokenDto> GetSavedRefreshTokens(string username, string refreshToken);
	}
}
