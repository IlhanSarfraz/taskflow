namespace TaskFlow.Application.Features.Boards.DTOs
{
    public sealed record BoardSummaryResponse(
        Guid Id,
        string Name,
        Guid ProjectId);
}
