using MediatR;
using TaskFlow.Application.Features.Notifications.DTOs;

namespace TaskFlow.Application.Features.Notifications.Queries.GetMyNotifications
{
    public sealed record GetMyNotificationsQuery : IRequest<List<NotificationResponse>>;
}