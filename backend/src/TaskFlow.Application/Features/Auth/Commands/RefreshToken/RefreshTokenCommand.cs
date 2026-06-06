using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Login;

namespace TaskFlow.Application.Features.Auth.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand()
        : IRequest<LoginResponse>;
}
