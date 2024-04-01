 

namespace blacklist.Application.Implementations.UserAccount
{
    public class AccountLogout : ResponseBaseService, IAccountLogout
    {
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountLogout> _logger;
        private readonly ISessionsService _sessionsService;
        private readonly IAppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly string _language;
        public AccountLogout(IMessageProvider messageProvider, IHttpContextAccessor httpContext, SignInManager<ApplicationUser> signInManager, ILogger<AccountLogout> logger, ISessionsService sessionsService, IConfiguration config, UserManager<ApplicationUser> userManager,
            IAppDbContext context, ITokenService tokenService) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));

            _logger = logger;
            _sessionsService = sessionsService;
            _config = config;
        
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
        }
        public async Task ClearsSession()
        {
            string cookie = string.Empty;
            var cookies = _httpContext.HttpContext.Request.Cookies;
            foreach (var ckie in cookies)
            {
                cookie = ckie.Key;
            }
            _logger.LogInformation("User has been logout successfully");
            await Task.Run(() => _httpContext.HttpContext.Response.Cookies.Delete(cookie));
        }

        public async Task<ServerResponse<bool>> LogOut()
        {
            var response = new ServerResponse<bool>();

            var token = _tokenService.GetToken();
            if (token == null)
            {
                return SetError(response, ResponseCodes.INVALID_TOKEN, _language );
            }
            var userId = token.Id;

            var logout = await _sessionsService.DeleteSessionAsync(userId, _language);
            
            if(logout != null)
            {

                response.Data = true;
                response.IsSuccessful = true;
                response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.SUCCESS, _language);

            }
            else
            {
                response.Data = false;
                response.IsSuccessful = false;

                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.REQUEST_NOT_SUCCESSFUL,
                    ResponseDescription = "Logout Failed"
                };
            }
            return response;
        }
    
    }
}
