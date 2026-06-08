namespace TaskFlow.Application.Features.Boards.DTOs
{
    public sealed record BoardResponse(
        Guid Id,
        string Name,
        Guid ProjectId);
}
