using MediatR;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.AddProjectMember
{
    public sealed record AddProjectMemberCommand(
        Guid ProjectId,
        Guid UserId,
        ProjectMemberRole Role)
        : IRequest;
}
