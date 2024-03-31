

using blacklist.Presentation.Controllers;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace VAT_SOP.Presentation.Controllers.Logging
{
    [Route("api/[controller]")]
    [Area("Logging")]
    public class TestsController : BaseAPIController
    {
        private readonly ILogger<TestsController> _logger;

        public TestsController(ILogger<TestsController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Get([FromHeader]string language)
        {
            Random rd = new Random(1000000000);
            for (int i = 0; i <= 20; i++)
            {
                long result = rd.Next(2, 999999);
                if (result > 300) throw new ArgumentNullException(  nameof(result));
                rd.Next();
                _logger.LogInformation("Check Kibana to text");
                _logger.LogInformation("Random {result}", result);
            }
            return Ok();
        }
    }
}
