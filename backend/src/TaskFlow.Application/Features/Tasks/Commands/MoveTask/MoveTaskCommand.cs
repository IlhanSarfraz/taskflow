using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask
{
    public sealed record MoveTaskCommand(
        Guid TaskId,
        Guid TargetColumnId)
        : IRequest;
}
