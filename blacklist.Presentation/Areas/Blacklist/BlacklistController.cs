using blacklist.Application.Interfacses.Blacklist;
using blacklist.Application.Interfacses.DashBoard;
using blacklist.Presentation.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace blacklist.Presentation.Areas.Blacklist
{
    [Authorize]
    [Area("Blacklist")]
    [Route("api/[controller]")]
    public class BlackListController : BaseAPIController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMemoryCache _memoryCache;
        private readonly MicsrosoftAADHelper _micsrosoftAAD;
        private readonly IBlacklistService _blacklistService;


        public BlackListController(ILogger<HomeController> logger, IUserService userService, SignInManager<ApplicationUser> signInManager, IMemoryCache memoryCache, MicsrosoftAADHelper micsrosoftAAD, IBlacklistService blacklistService)
        {
            _logger = logger;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _signInManager = signInManager;
            _memoryCache = memoryCache;
            _micsrosoftAAD = micsrosoftAAD;
            _blacklistService = blacklistService;

        }


        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("blacklist-user-category")]
        public async Task<IActionResult> BlackListUserBycategory([FromBody] BlacklistDto request)
        {
            var response = await _blacklistService.CreateBlacklistByCategory(request);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("blacklist-user-email")]
        public async Task<IActionResult> BlackListUserByEmail([FromBody] BlackListUserDto request)
        {
            var response = await _blacklistService.CreateBlacklistByEmail(request);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("read-blacklisted-by-email")]
        public async Task<IActionResult> GetBlackListedUserByEmail(string email)
        {
            var response = await _blacklistService.GetBlacklistedUserByEmail(email);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpGet("read-all-blacklisted-user")]
        public async Task<IActionResult> GetAllBlackListedUserByEmail()
        {
            var response = await _blacklistService.GetAllBlasklistedUsers();

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("read-blacklisted-category")]
        public async Task<IActionResult> GetBlackListedUserByCategory(string category)
        {
            var response = await _blacklistService.GetBlacklistedUserByCategory(category);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("delete-blacklist-email")]
        public async Task<IActionResult> DeleteBlacklistByEmail(string email)
        {
            var response = await _blacklistService.DeleteBlacklistUser(email);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("delete-blacklist-category")]
        public async Task<IActionResult> DeleteBlacklistByCategory([FromBody] BlacklistCategory category)
        {
            var response = await _blacklistService.DeleteBlacklistedUsers(category);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<BlacklistDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPut("update-blacklist")]
        public async Task<IActionResult> UpdateBlacklist([FromBody] BlacklistDto request)
        {
            var response = await _blacklistService.UpdateBlacklist(request);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

    }
}
