namespace TaskFlow.Application.Features.Auth.DTOs.Project
{
    public sealed record ProjectResponse(
        Guid Id,
        string Name,
        string Key,
        string Description,
        Guid OwnerId,
        DateTime CreatedAt);
}
