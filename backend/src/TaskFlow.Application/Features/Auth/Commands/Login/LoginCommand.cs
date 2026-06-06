using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Login;

namespace TaskFlow.Application.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password)
        : IRequest<LoginResponse>;
}
