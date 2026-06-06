using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Auth.Commands.Register;
using TaskFlow.Application.Features.Auth.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Auth.Handlers
{
    public sealed class RegisterHandler :
        IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterResponse> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            bool exists = await _context.Users
                .AnyAsync(
                x => x.Email == request.Email,
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Email already exists.");
            }

            User user = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash =
                    _passwordHasher.Hash(request.Password)
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync(
                cancellationToken);

            return new RegisterResponse(
                user.Id,
                user.Email);
        }
    }
}
