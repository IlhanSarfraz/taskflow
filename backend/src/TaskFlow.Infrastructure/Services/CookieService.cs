using Microsoft.AspNetCore.Http;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CookieService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void SetRefreshToken(string token)
        {
            CookieOptions options = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            _contextAccessor.HttpContext?.Response
                .Cookies.Append("refreshToken", token, options);

        }

        public string? GetRefreshToken()
        {
            return _contextAccessor.HttpContext?.Request
                .Cookies["refreshToken"];
        }
    }
}
