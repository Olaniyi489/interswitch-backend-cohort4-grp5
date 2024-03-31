using Microsoft.AspNetCore.Mvc;

namespace blacklist.Presentation.Controllers
{
    [ApiExplorerSettings(IgnoreApi =true)]
    [Route("api/[controller]")]
    public class AccessDeniedController : BaseAPIController
    {
        [HttpGet("access")]
        public IActionResult Access()
        {
            return Ok();
        }
    }
}
