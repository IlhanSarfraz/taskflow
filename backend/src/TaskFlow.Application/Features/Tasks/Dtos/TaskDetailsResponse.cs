using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Dtos
{
    public sealed record TaskDetailsResponse(
        Guid Id,
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate,
        Guid ProjectId,
        Guid BoardColumnId);
}
