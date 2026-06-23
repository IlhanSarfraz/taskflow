using MediatR;

namespace TaskFlow.Application.Features.Projects.Commands.DeclineInvite
{
    public sealed record DeclineInviteCommand(Guid InviteId) : IRequest;
}