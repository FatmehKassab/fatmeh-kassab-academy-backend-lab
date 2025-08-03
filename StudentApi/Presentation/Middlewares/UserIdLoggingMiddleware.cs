using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DefaultNamespace.Middlewares
{
    public class UserIdLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserIdLoggingMiddleware> _logger;

        public UserIdLoggingMiddleware(RequestDelegate next, ILogger<UserIdLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Hello incoming requesting");
            await _next(context);
        }
    }

    public static class UserIdLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserIdLoggingMiddleware>();
        }
    }
}