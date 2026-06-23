using MediatR;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.CreateInvite
{
    public sealed record CreateInviteCommand(
        Guid ProjectId,
        Guid InvitedUserId,
        ProjectMemberRole Role)
        : IRequest;
}