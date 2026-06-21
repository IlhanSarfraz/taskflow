using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.RefreshToken;
using TaskFlow.Application.Features.Auth.DTOs.Login;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Auth.Handlers
{
    public sealed class RefreshTokenHandler
    {
        private readonly IApplicationDbContext _context;
        private readonly ICookieService _cookieService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshTokenHandler(
            IApplicationDbContext context,
            ICookieService cookieService,
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService)
        {
            _context = context;
            _cookieService = cookieService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<LoginResponse> Handle(
            RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            string? refreshToken = _cookieService.GetRefreshToken()
                ?? throw new UnauthorizedAccessException("Refresh token is missing.");

            RefreshToken? tokenEntity = await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                x => x.Token == refreshToken,
                cancellationToken);
            if (tokenEntity == null || !tokenEntity.IsActive)
                throw new Exception("Invalid refresh token");

            User user = tokenEntity.User;

            string newRefreshToken = _refreshTokenService.Generate();

            tokenEntity.Token = newRefreshToken;
            tokenEntity.ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
            tokenEntity.CreatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _cookieService.SetRefreshToken(newRefreshToken);

            string newJwt = _jwtTokenService.GeneratedToken(user);

            return new LoginResponse(
                newJwt,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName
            );
        }
    }
}
