using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteTask
{
    public sealed record DeleteTaskCommand(Guid TaskId)
        : IRequest;
}
