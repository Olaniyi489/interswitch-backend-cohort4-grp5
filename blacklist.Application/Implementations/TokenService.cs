
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace blacklist.Application.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IAppDbContext _context;
        private readonly ILogger<TokenService> _log;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly string _token;


        public TokenService(IConfiguration iconfiguration, IAppDbContext context, ILogger<TokenService> log, IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _configuration = iconfiguration ?? throw new ArgumentNullException(nameof(_configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            _log = log;
            _httpContext = httpContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _token = _httpContext.HttpContext.GetTokenAsync("access_token")?.GetAwaiter().GetResult();
        }
        public async Task<Tokens> GenerateToken(UserDto user)
        {
            return await GenerateJWTTokens(user);
        }

        public async Task<Tokens> GenerateRefreshToken(UserDto user)
        {
            return await GenerateJWTTokens(user);
        }

        public async Task<Tokens> GenerateJWTTokens(UserDto user)
        {
            try
            {
                var data = user;
                if (data is null)
                {
                    return null;
                }

                if(string.IsNullOrWhiteSpace(user.Email)) 
                {
                    return null;
                }

                //var userPermissions = await _context.GetData<GetPermissionModel>("Exec SP_GetUserPermissions", new SqlParameter("@mail", user.Email));
                // Assuming _context is your DbContext instance
                var userPermissions = await _context.Users
                .Where(u => u.Email == user.Email)
                .Join(
                    _context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole }
                )
                .Join(
                    _context.RolePermissions,
                    ur => ur.userRole.RoleId,
                    rolePermission => rolePermission.RoleId,
                    (ur, rolePermission) => new { ur.user, ur.userRole, rolePermission }
                )
                .Join(
                    _context.Permissions,
                    urp => urp.rolePermission.PermissionId,
                    permission => permission.Id,
                    (urp, permission) => new GetPermissionModel
                    {
                        PermissionName = permission.Name,
                        RoleId = urp.rolePermission.RoleId
                    }
                )
                .ToListAsync();
                var userPermissionsSerialize = JsonConvert.SerializeObject(userPermissions);
                // Else we generate JSON Web Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = _configuration["ApplicationSettings:SecurityKey"];
                var tokenKey = Encoding.UTF8.GetBytes(key);
                //var expirationMinutes = _configuration["jwt:TimeoutMinutes"];
                var expirationMinutes = 10;
                var subject = new ClaimsIdentity(new Claim[]
                      {
                             new Claim(ClaimTypes.Name, data.UserName),
                             new Claim(ClaimTypes.Email, data.Email),
                             //new Claim(ClaimTypes.MobilePhone, data.PhoneNumber),
                             new Claim("UserPermissions", userPermissionsSerialize),
                             new Claim ("User", JsonConvert.SerializeObject(user)),
                             
                             new Claim("Id", user.Id)
                      });
                var signature = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(expirationMinutes)),
                    SigningCredentials = signature
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = GenerateRefreshToken();
                await Task.CompletedTask;
                return new Tokens { Token = tokenHandler.WriteToken(token), RefreshToken = refreshToken };

            }
            catch (Exception ex)
            {
                //Add Logging

            }
            await Task.CompletedTask;
            return null;

        }

        public TokenConvert GetToken(string tok = "")
        {


            var claimsIdentity = _httpContext.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                IEnumerable<Claim> claims = claimsIdentity.Claims;
                if (claims != null && claims.Count() > 0)
                {
                    string userDetails = claimsIdentity.FindFirst(p => p.Type == "User")?.Value;
                    string name = claimsIdentity.FindFirst(p => p.Type == "Name")?.Value;
                    string email = claimsIdentity.FindFirst(p => p.Type == "Email")?.Value;
                    string userId = claimsIdentity.FindFirst(p => p.Type == "UserId")?.Value;
                    string userPermission = claimsIdentity.FindFirst(p => p.Type == "UserPermissions")?.Value;

                    var user = claimsIdentity.FindFirst("User")?.Value;

                    var userPermissions = !string.IsNullOrWhiteSpace(userPermission) ? JsonConvert.DeserializeObject<List<GetPermissionModel>>(userPermission) : new List<GetPermissionModel>();
                    var userDetailsDeserialize = !string.IsNullOrWhiteSpace(userDetails) ? JsonConvert.DeserializeObject<UserDto>(userDetails) : new UserDto();
                    return new TokenConvert
                    {
                        UserName = name,
                        Email = email,
                        Id = userId,
                        UserPermissions = userPermissions,
                        User = userDetailsDeserialize
                        
                    };
                }
                else
                {
                    var token = !string.IsNullOrWhiteSpace(tok) ? tok : _token;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return null;
                    }
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(token);

                    if (jwtSecurityToken != null)
                    {
                        var claimsIdentitys = jwtSecurityToken.Claims;
                        // or
                        string userDetails = claimsIdentitys.FirstOrDefault(p => p.Type == "User")?.Value;
                        string name = claimsIdentitys.FirstOrDefault(p => p.Type == "Name")?.Value;
                        string email = claimsIdentitys.FirstOrDefault(p => p.Type == "Email")?.Value;
                        string userId = claimsIdentitys.FirstOrDefault(p => p.Type == "UserId")?.Value;
                        string userPermission  = claimsIdentitys.FirstOrDefault(p => p.Type == "UserPermissions")?.Value;

                        var user = claimsIdentity.FindFirst("User")?.Value;

                        var userPermissions = !string.IsNullOrWhiteSpace(userPermission) ? JsonConvert.DeserializeObject<List<GetPermissionModel>>(userPermission) : new List<GetPermissionModel>();
                        var userDetailsDeserialize = !string.IsNullOrWhiteSpace(userDetails) ? JsonConvert.DeserializeObject<UserDto>(userDetails) : new UserDto();

                        return new TokenConvert 
                        { 
                            UserName = name,
                            Email = email,
                            Id = userId,
                            UserPermissions = userPermissions,
                            User = userDetailsDeserialize
                        };
                    }

                }
            }

            return new TokenConvert();
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:SecurityKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = _configuration.GetValue<bool>("Jwt:ValidateSigningKey"),
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ValidateIssuer = _configuration.GetValue<bool>("Jwt:ValidateIssuer"),
                ValidIssuer = _configuration.GetValue<string>("Jwt:Issuer"),
                ValidateAudience = _configuration.GetValue<bool>("Jwt:ValidateAudience"),
                ValidAudience = _configuration.GetValue<string>("Jwt:Audience"),
                ValidateLifetime = _configuration.GetValue<bool>("Jwt:ValidateLifeTime"),
                ClockSkew = TimeSpan.FromMinutes(_configuration.GetValue<int>("Jwt:DateToleranceMinutes"))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken ?? throw new ArgumentNullException(nameof(securityToken));
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }


            return principal;
        }

        public async Task<bool> AddUserRefreshTokens(UserRefreshTokenDto request)
        {
            var data = request.Adapt<UserRefreshToken>();        
            await _context.UserRefreshTokens.AddAsync(data);
            var saved = await _context.SaveChangesAsync();
            return saved > 0;

        }
        public async Task DeleteUserRefreshTokens(string username, string refreshToken)
        {
            var item = await _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _context.UserRefreshTokens.Remove(item);
            }
        }
        public async Task<UserRefreshTokenDto> GetSavedRefreshTokens(string username, string refreshToken)
        {
            var data = await _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken.Equals(refreshToken) && x.IsActive.Equals(true));
            if (data != null) { return data.Adapt<UserRefreshTokenDto>(); }
            else { return new UserRefreshTokenDto(); }
        }
    }
}
