
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Tavis.UriTemplates;

using blacklist.Presentation.Controllers;

namespace VAT_SOP.Presentation.Controllers.UserAccounts
{
    [Authorize]
    [Area("UserAccounts")]
    [Route("api/[controller]")]
    public class UsersController : BaseAPIController 
    {
        private readonly IUserService _userService;
      

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
           
        }

        [ProducesResponseType(typeof(ServerResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("view-users")]
        public async Task<IActionResult> GetUsers(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();
            var response = await _userService.GetProfile(userId);


            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("view-all-users")]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userService.GetAllUsers();

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("create-user")]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.CreateUserAsync(model);

            if (response.IsSuccessful)
            {
                return Ok( response );
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("view-user-by-role")]
        public async Task<IActionResult> GetUsersByRole([FromQuery] string roleName)
        {
            var response = await _userService.GetAllUserByRole(roleName);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("deactivate-user")]
        public async Task<IActionResult> DisableUserProfile(string userEmail)
        {
            var response = await _userService.DisableUserProfile(userEmail);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("activate-user")]
        public async Task<IActionResult> ActivateUserProfile(string userEmail)
        {
            var response = await _userService.ActivateUserProfile(userEmail);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("update-user-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUser request)
        {
            var response = await _userService.UpdateProfile(request);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }
        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("delete-user-profile")]
        public async Task<IActionResult> DeleteUserProfile(string email)
        {
            var response = await _userService.DeleteUserProfile(email);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<List<UserDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("IsUserExist")]
        public async Task<IActionResult> IsUserExists(string email)
        {
            var response = await _userService.IsUserExists(email);

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
