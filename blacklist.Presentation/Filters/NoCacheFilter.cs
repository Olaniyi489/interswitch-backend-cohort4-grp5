
namespace blacklist.Presentation.Filters
{
    
    public  class NoCacheFilter : IAsyncActionFilter
    {
		public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
		{

            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";

          await next();
        }

		 
    }
}
