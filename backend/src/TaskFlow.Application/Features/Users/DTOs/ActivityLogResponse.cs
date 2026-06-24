namespace TaskFlow.Application.Features.Users.DTOs
{
    public sealed record ActivityLogResponse(
        Guid Id,
        string Action,
        string EntityType,
        Guid EntityId,
        string Description,
        DateTime CreatedAtUtc);
}
