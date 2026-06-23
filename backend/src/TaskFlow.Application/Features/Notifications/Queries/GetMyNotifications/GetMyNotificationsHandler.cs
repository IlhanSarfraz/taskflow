using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Notifications.DTOs;

namespace TaskFlow.Application.Features.Notifications.Queries.GetMyNotifications
{
    public sealed class GetMyNotificationsHandler
        : IRequestHandler<GetMyNotificationsQuery, List<NotificationResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetMyNotificationsHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<NotificationResponse>> Handle(
            GetMyNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == _currentUser.UserId)
                .OrderByDescending(n => n.CreatedAtUtc)
                .Take(30)
                .Select(n => new NotificationResponse(
                    n.Id,
                    n.Type.ToString(),
                    n.Title,
                    n.Message,
                    n.RelatedEntityId,
                    n.IsRead,
                    n.CreatedAtUtc))
                .ToListAsync(cancellationToken);
        }
    }
}