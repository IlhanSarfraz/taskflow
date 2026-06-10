namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask
{
    public sealed record MoveTaskRequest(
        Guid TargetColumnId);
}
