using MediatR;

namespace TaskFlow.Application.Features.Projects.Commands.RemoveProjectMember
{
    public sealed record RemoveProjectMemberCommand(
        Guid ProjectId,
        Guid UserId)
        : IRequest;
}
