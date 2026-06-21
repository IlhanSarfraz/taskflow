namespace TaskFlow.Application.Features.Tasks.Dtos
{
    public sealed record AssignTaskRequest(IReadOnlyList<Guid> AssigneeIds);
}
