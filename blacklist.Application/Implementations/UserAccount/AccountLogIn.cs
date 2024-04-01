

namespace blacklist.Application.Implementations.UserAccount
{
    public class AccountLogIn : IAccountLogIn
    {
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountLogout> _logger;
        private readonly ISessionsService _sessionsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly int loginCount = 0;
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly string _language;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountLogIn(
            IMessageProvider messageProvider,
            IHttpContextAccessor httpContext,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ITokenService tokenService,
            ILogger<AccountLogout> logger,
            ISessionsService sessionsService, IAppDbContext context, IConfiguration config)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessionsService = sessionsService ?? throw new ArgumentNullException(nameof(sessionsService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            loginCount = _config.GetValue<int>("LoginCount:Count");
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _roleManager = roleManager;
        }

        public async Task<ServerResponse<LogInResponse>> LogIn(string email, string password)
        {
            var response = new ServerResponse<LogInResponse>();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                response.IsSuccessful = false;
                response.Data = null;
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.RECORD_DOES_NOT_EXISTS,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.RECORD_DOES_NOT_EXISTS, _language)
                };

                return response;
            }

            if (user.IsBlacklisted)
            {
                response.IsSuccessful = false;
                response.Data = null;
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.RECORD_DOES_NOT_EXISTS,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.USER_IS_BLACKLISTED, _language)
                };

                return response;
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (signInResult != null && signInResult.Succeeded)
            {
                UserDto userDto = user.Adapt<UserDto>();

                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles != null && userRoles.Any())
                {
                    var roleName = userRoles.First();
                    var role = await _roleManager.FindByNameAsync(roleName);

                    if (role != null)
                    {
                        var roleId = role.Id;
                    }

                    var rolePermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == role.Id)
                        .Select(rp => rp.Permission)
                        .ToListAsync();


                    var tokens = await _tokenService.GenerateToken(userDto);

                    var sessionRequest = new SessionDTO
                    {
                        DateCreated = DateTime.Now,
                        Token = tokens.Token,
                        UserId = userDto.Id,
                    };

                    var sessionResponse = await _sessionsService.CreateSessionAsync(sessionRequest, _language);
                    if (sessionResponse != null && sessionResponse.IsSuccessful)
                    {
                        response.IsSuccessful = true;

                        response.Data = new LogInResponse
                        {
                            User = userDto,
                            RoleId = role.Id,
                            RoleName = role.Name,
                            Token = tokens,
                            Permissions = rolePermissions.AsQueryable().ProjectToType<PermissionDTO>().ToList()
                        };

                        response.SuccessMessage = "User login successfully";
                        response.IsSuccessful = true;
                    }
                    else
                    {
                        response.IsSuccessful = false;
                        response.Data = null;
                        response.Error = sessionResponse.Error;
                        return response;
                    }
                }
            }
            else
            {
                response.IsSuccessful = false;
                response.Data = null;
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.UNAUTHORIZED,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.UNAUTHORIZED, _language)
                };
            }

            return response;
        }

        public async Task<ServerResponse<bool>> CheckLoginCount(string language, string email)
        {
            var response = new ServerResponse<bool>();
            if (string.IsNullOrWhiteSpace(email))
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_PARAMETER,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.INVALID_PARAMETER, language)
                };
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_PARAMETER,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.INVALID_PARAMETER, language)
                };
            }
            bool isEqual = await _context.Users.AnyAsync(p => p.LoginCount.Equals(loginCount));
            response.Data = isEqual;
            response.IsSuccessful = true; return response;

        }
    }
}
