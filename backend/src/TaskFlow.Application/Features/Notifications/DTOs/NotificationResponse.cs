namespace TaskFlow.Application.Features.Notifications.DTOs
{
    public sealed record NotificationResponse(
        Guid Id,
        string Type,
        string Title,
        string Message,
        Guid? RelatedEntityId,
        bool IsRead,
        DateTime CreatedAtUtc);
}