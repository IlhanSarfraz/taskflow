using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.ChangePassword;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Auth.Handlers
{
    public sealed class ChangePasswordHandler
        : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _currentUser = currentUser;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == _currentUser.UserId,
                cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            bool isCurrentPasswordValid = _passwordHasher.Verify(
                request.CurrentPassword,
                user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new UnauthorizedAccessException(
                    "Current password is incorrect.");
            }

            user.PasswordHash =
                _passwordHasher.Hash(request.NewPassword);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
