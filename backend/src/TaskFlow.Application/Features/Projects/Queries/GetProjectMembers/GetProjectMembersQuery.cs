using MediatR;
using TaskFlow.Application.Features.Projects.DTOs;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectMembers
{
    public sealed record GetProjectMembersQuery(
        Guid ProjectId)
        : IRequest<List<ProjectMemberResponse>>;
}