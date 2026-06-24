using MediatR;

namespace TaskFlow.Application.Features.Auth.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword)
        : IRequest;
}

