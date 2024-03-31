


using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Web.Resource;

namespace blacklist.Presentation.Controllers.UserAccounts
{
    [ApiExplorerSettings(IgnoreApi = true)]
   [Area("UserAccounts")]
    [Route("api/[controller]")]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:ScopeName")]
    [Authorize]
	public class AuthenticationController : BaseAPIController
	{
		private readonly IUserService _userService;
		private readonly IAccountLogout _logout;
		private readonly IAccountLogIn _logIn;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ClaimsHelper _claimsHelper;
        private readonly IConfiguration _config;
		private readonly IntegratedWindowsTokenProvider _provider;
        public AuthenticationController(
            IUserService userService,
            ISessionsService session,
            IAccountLogIn logIn,
            IAccountLogout logout,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config,
            ClaimsHelper claimsHelper,
            IntegratedWindowsTokenProvider provider)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logout = logout ?? throw new ArgumentNullException(nameof(logout));
            _logIn = logIn ?? throw new ArgumentNullException(nameof(logIn));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager;
            _config = config;
            _claimsHelper = claimsHelper;
            _provider = provider;
        }



        [HttpGet("get-user")]
		public async Task<IActionResult> GetUser()
		{

            HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authorizationToken);
            string email = User.Identity.Name;
			var user = await _userManager.FindByNameAsync(email);
		if (user == null)
			{ 
                return Ok();
			}
			else
			{
				return BadRequest("Add data");

			} 

		}
        [HttpPost("aurthcode")]
        public async Task<IActionResult> GetAuthCode()
        {



			return Ok();

        }


        [HttpPost("update")]
		public async Task<IActionResult> UpdateUser(UpdateUser request)

		{
			var response = await _userService.UpdateProfile(request);
			if (response.IsSuccessful)
			{
				return RedirectToAction("Index", "Home");
			}

			return Ok();


		}


		[HttpGet("signe-out")]
		public async Task<IActionResult> SignOut(UpdateUser request)

		{
			await _signInManager.SignOutAsync();

			return Ok("Add data");


		}

        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var res = await _userService.DeleteUserProfile(email);
            if (res.IsSuccessful)
            {
                return Ok(res);
            }
            return BadRequest("Add data");
        }

        [HttpPost("disable-user")]
        public async Task<IActionResult> DisableUser(string email)
        {
            var res = await _userService.DisableUserProfile(email);
            if (res.IsSuccessful)
            {
                return Ok(res);
            }
            return BadRequest("Add data");
        }

        [HttpGet("user-exists")]
        public async Task<IActionResult> IsUserExists(string email)
        {
            var res = await _userService.IsUserExists(email);
            if (res.IsSuccessful)
            {
                return Ok(res);
            }
            return BadRequest("Add data");
        }

        [HttpGet("user-profile")]
        public async Task<IActionResult> GetProfile(string email)
        {
            var res = await _userService.GetProfile(email);
            if (res.IsSuccessful)
            {
                return Ok(res);
            }
            return BadRequest("Add data");
        }

    }
}
