using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Project;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjects
{
    public sealed record GetProjectsQuery
        : IRequest<List<ProjectResponse>>;

}
