using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Project;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectById
{
    public sealed record GetProjectByIdQuery(Guid Id)
        : IRequest<ProjectResponse>;
}
