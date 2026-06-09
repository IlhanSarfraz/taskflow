using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Dtos
{
    public sealed record TaskResponse(
        Guid Id,
        string Title,
        string? Description,
        TaskPriority Priority,
        DateTime? DueDate,
        Guid ProjectId,
        Guid BoardColumnId);
}
