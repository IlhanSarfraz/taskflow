using MediatR;
using TaskFlow.Application.Features.Auth.DTOs.Project;

namespace TaskFlow.Application.Features.Auth.Commands.CreateProject
{
    public sealed record CreateProjectCommand(
        string Name,
        string Key,
        string Description)
        : IRequest<ProjectResponse>;
}
