



using System.Net;
using blacklist.Application.Interfacses;

namespace blacklist.Presentation.Filters
{
    public class LanguageFilter : IAsyncActionFilter
    {
        private readonly IMessageProvider _messageProvider;
        private readonly ILogger<LanguageFilter> _logger;
        public LanguageFilter(IMessageProvider messageProvider, ILogger<LanguageFilter> logger)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasLanguage = context.HttpContext.Request.Headers.TryGetValue(ResponseCodes.LANGUAGE,out var language);
            if (hasLanguage)
            {
              await  next();
            }
            else
            {
                context.Result = new ObjectResult(
                    new ErrorResponse
                    {
                        ResponseCode = ResponseCodes.REQUIRE_COUNTRY_FLAG,
                        ResponseDescription = _messageProvider.GetMessage(ResponseCodes.REQUIRE_COUNTRY_FLAG, ResponseCodes.DEFAULT_LANGUAGE)
                    })
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            return;
            }
        }
    }
}
