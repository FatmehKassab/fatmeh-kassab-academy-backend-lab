using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DefaultNamespace.Filters
{
    public class CallerHeaderFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var hasHeader = context.HttpContext.Request.Headers.TryGetValue("caller", out var caller);
            if (!hasHeader || caller == "Unknown")
            {
                context.Result = new ContentResult
                {
                    StatusCode = 400,
                    Content = "Request rejected: Invalid or missing 'caller' header."
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing after the action
        }
    }
}