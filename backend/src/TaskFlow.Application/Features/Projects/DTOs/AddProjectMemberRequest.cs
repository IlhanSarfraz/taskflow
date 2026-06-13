using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.DTOs
{
    public sealed record AddProjectMemberRequest(
        Guid UserId,
        ProjectMemberRole Role);

}
