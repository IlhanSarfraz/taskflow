using MediatR;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.CreateInvite
{
    public sealed record CreateInviteCommand(
        Guid ProjectId,
        string InvitedEmail,
        ProjectMemberRole Role)
        : IRequest;
}