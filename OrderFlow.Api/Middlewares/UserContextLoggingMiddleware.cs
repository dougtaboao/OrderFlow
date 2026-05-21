using Serilog.Context;
using OrderFlow.Application.Security;

namespace OrderFlow.Api.Middlewares
{
    public class UserContextLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ICurrentUser currentUser)
        {
            using (LogContext.PushProperty("UserId", currentUser.UserId))
            using (LogContext.PushProperty("UserName", currentUser.Name))
            using (LogContext.PushProperty("Role", currentUser.Role))
            {
                await _next(context);
            }
        }
    }
}