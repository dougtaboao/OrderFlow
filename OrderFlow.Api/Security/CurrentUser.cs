using System.Security.Claims;
using OrderFlow.Application.Security;

namespace OrderFlow.Api.Security
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return Guid.TryParse(value, out var id)
                    ? id
                    : Guid.Empty;
            }
        }

        public string Name =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.Name)?
                .Value
            ?? string.Empty;

        public string Role =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.Role)?
                .Value
            ?? string.Empty;

        public bool IsAuthenticated =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .Identity?
                .IsAuthenticated
            ?? false;
    }
}