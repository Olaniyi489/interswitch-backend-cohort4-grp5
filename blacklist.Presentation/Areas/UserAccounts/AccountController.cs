using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tavis.UriTemplates;
using blacklist.Application.Common.Models;
using blacklist.Presentation.Controllers;

namespace blacklist.Presentation.Areas.UserAccounts
{
    [Area("UserAccounts")]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : BaseAPIController
    {
        private readonly IAccountLogIn _accountLogIn;
        private readonly IAccountLogout _accountLogout;
        private readonly IResetPassword _resetPassword;
        private readonly IUserService _userService;
       
        public AccountController(IAccountLogIn accountLogIn, IAccountLogout accountLogout, IResetPassword resetPassword, IUserService userService)
        {
            _accountLogIn = accountLogIn;
            _accountLogout = accountLogout;
            _resetPassword = resetPassword;
            _userService = userService;
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ServerResponse<LogInResponse>), StatusCodes.Status200OK)]
       // [ProducesErrorResponseType(typeof(ErrorResponse))]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LogInRequest model)
        {
            var result = await _accountLogIn.LogIn(model.Email, model.Password);

            if (result.IsSuccessful)
            {
                return Ok( result );
            }

            return Unauthorized( result.Error);
        }

        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _accountLogout.LogOut();

            if (result.IsSuccessful)
            {
                return Ok(  result );
            }

            return BadRequest( result .Error);
        }

        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("activate-account/{Id}")]
        public async Task<IActionResult> ActivateAccount(string id)
        {
            var response = await _userService.ActivateAccount(id);
            if (response.IsSuccessful)
            {
                return Ok( response );
            }

            return BadRequest( response.Error);
        }

        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("change-password/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordDTO model)
        {
            var response = await _resetPassword.ChangePassword(userId, model);

            if (response.IsSuccessful)
            {
                return Ok(  response );
            }

            return BadRequest(  response .Error);
        }

        [ProducesResponseType(typeof(ServerResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("password-reset-otp")]
        public async Task<IActionResult> PasswordResetOTP(string email)
        {
            var response = await _resetPassword.GeneratePasswordResetOTP(email);

            if (response.IsSuccessful)
            {
                return Ok(  response );
            }

            return BadRequest(  response.Error);
        }
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(OTPDto model)
        {
            var response = await _resetPassword.ResetPasswordWithOTP(model);

            if (response.IsSuccessful)
            {
                return Ok(  response );
            }

            return BadRequest(  response .Error);
        }

        
    }
}
