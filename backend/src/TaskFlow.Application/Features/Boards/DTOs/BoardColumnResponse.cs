using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Boards.DTOs
{
    public sealed record BoardColumnResponse(
        Guid Id,
        string Name,
        int Order,
        IReadOnlyCollection<TaskSummaryResponse> Tasks);
}
