using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Auth
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public Guid UserId
        {
            get
            {
                string? value = User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return Guid.TryParse(value, out Guid id)
                    ? id
                    : Guid.Empty;
            }
        }

        public string Email =>
            User?
            .FindFirst(ClaimTypes.Email)?
            .Value
            ?? string.Empty;

        public string Role =>
            User?
            .FindFirst(ClaimTypes.Role)?
            .Value
            ?? string.Empty;
    }
}
