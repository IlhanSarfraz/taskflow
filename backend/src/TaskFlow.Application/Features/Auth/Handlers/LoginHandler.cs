using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.Login;
using TaskFlow.Application.Features.Auth.DTOs.Login;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Auth.Handlers
{
    public sealed class LoginHandler :
        IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICookieService _cookieService;

        public LoginHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService,
            ICookieService cookieService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _cookieService = cookieService;
        }

        public async Task<LoginResponse> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(
                x => x.Email == request.Email,
                cancellationToken);

            if (user is null ||
                !_passwordHasher.Verify(
                    request.Password,
                    user.PasswordHash))
            {
                throw new InvalidOperationException(
                    "Invalid email or password.");
            }

            string refreshToken = _refreshTokenService.Generate();

            RefreshToken refreshTokenEntity = new()
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync(cancellationToken);

            _cookieService.SetRefreshToken(refreshToken);

            string token = _jwtTokenService.GeneratedToken(user);

            return new LoginResponse(
                token,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName);
        }
    }
}
