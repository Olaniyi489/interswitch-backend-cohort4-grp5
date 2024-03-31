using blacklist.Presentation.Controllers;

namespace VAT_SOP.Presentation.Controllers
{
    [Area("UserAccounts")]
    [Route("api/[controller]")]
    [NonController]
    public class TokenController : BaseAPIController
    {
        private readonly ITokenService _token;
        public TokenController(ITokenService token)
        {
            _token = token;
        }
        [ProducesResponseType(typeof(Tokens), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken(UserDto request)
        {

            var token = await _token.GenerateToken(request);
             
                return Ok(token);
            
        } 
        [HttpPost("generate-refresh-token")]
        public async Task<IActionResult> GenereteRefeshToken(UserDto request)
        {

            var token = await _token.GenerateRefreshToken(request);
             
                return Ok(token);
            
        }
    }
}
