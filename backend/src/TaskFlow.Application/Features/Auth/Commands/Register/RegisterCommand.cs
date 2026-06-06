using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Register;

namespace TaskFlow.Application.Features.Auth.Commands.Register
{
    public sealed record RegisterCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password)
        : IRequest<RegisterResponse>;
}
