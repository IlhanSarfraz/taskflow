using MediatR;

namespace TaskFlow.Application.Features.Notifications.Commands.MarkNotificationRead
{
    public sealed record MarkNotificationReadCommand(Guid NotificationId) : IRequest;
}