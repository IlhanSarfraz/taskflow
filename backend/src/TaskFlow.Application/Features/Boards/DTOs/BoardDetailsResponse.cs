namespace TaskFlow.Application.Features.Boards.DTOs
{
    public sealed record BoardDetailsResponse(
        Guid Id,
        string Name,
        Guid ProjectId,
        IReadOnlyCollection<BoardColumnResponse> Column);
}
