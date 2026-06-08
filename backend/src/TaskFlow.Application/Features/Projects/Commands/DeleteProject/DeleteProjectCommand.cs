using MediatR;

namespace TaskFlow.Application.Features.Projects.Commands.DeleteProject
{
    public sealed record DeleteProjectCommand(Guid Id)
        : IRequest;
}
