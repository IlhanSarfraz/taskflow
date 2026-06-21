using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Dtos
{
    public sealed record TaskSummaryResponse(
        Guid Id,
        string Title,
        TaskPriority Priority,
        DateTime? DueDate,
        string? AssigneeInitials);
}
