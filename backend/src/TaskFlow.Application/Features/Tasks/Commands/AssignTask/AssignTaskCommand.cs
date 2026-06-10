using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.AssignTask
{
    public sealed record AssignTaskCommand(
        Guid TaskId,
        Guid AssigneeId)
        : IRequest;
}
