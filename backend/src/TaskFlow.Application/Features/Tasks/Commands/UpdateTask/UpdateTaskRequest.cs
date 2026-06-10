using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public sealed record UpdateTaskRequest(
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate);
}
