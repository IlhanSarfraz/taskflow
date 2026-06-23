using MediatR;

namespace TaskFlow.Application.Features.Projects.Commands.AcceptInvite
{
    public sealed record AcceptInviteCommand(Guid InviteId) : IRequest;
}