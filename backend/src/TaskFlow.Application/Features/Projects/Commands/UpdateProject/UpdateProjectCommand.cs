using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Project;

namespace TaskFlow.Application.Features.Projects.Commands.UpdateProject
{
    public sealed record UpdateProjectCommand(
        Guid Id,
        string Name,
        string Description)
         : IRequest<ProjectResponse>;
}
