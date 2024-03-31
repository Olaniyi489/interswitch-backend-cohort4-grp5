using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using RestSharp;
using System.Diagnostics;
using System.Security.Claims;
using blacklist.Application.Interfacses.DashBoard;
using blacklist.Presentation.Controllers;

namespace blacklist.Presentation.Controllers
{
    [Authorize]
    [Area("DashBoards")]
    [Route("api/[controller]")]
    public class HomeController : BaseAPIController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMemoryCache _memoryCache;
        private readonly MicsrosoftAADHelper _micsrosoftAAD;
        private readonly IDashBoardService _dashBoard;

        public HomeController(ILogger<HomeController> logger, IUserService userService, SignInManager<ApplicationUser> signInManager, IMemoryCache memoryCache, MicsrosoftAADHelper micsrosoftAAD, IDashBoardService dashBoard)
		{
			_logger = logger;
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
			_signInManager = signInManager;
			_memoryCache = memoryCache;
			_micsrosoftAAD = micsrosoftAAD;
            _dashBoard = dashBoard;
        }

        [ProducesResponseType(typeof(ServerResponse<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
       
        [HttpGet("landing")]
		public async Task<IActionResult> Landing(string code,string state,string error)
       {
            if (User?.Identity?.IsAuthenticated == true)
            {
                		 

 
				return Ok("Pass the usermodel responsde here");
            }

            return Ok();
        }

        [ProducesResponseType(typeof(ServerResponse<DashBoardDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("dashboard-statistics")]
        public async Task<IActionResult> DashBoardStatistics(string roleId)
        {
            var response = await _dashBoard.GetDashDoardDetails(roleId);

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