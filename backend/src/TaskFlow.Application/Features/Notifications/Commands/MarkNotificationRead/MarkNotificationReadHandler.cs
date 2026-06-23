using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Notifications.Commands.MarkNotificationRead
{
    public sealed class MarkNotificationReadHandler
        : IRequestHandler<MarkNotificationReadCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public MarkNotificationReadHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            MarkNotificationReadCommand request,
            CancellationToken cancellationToken)
        {
            Notification? notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == request.NotificationId &&
                    n.UserId == _currentUser.UserId,
                    cancellationToken);

            if (notification is null)
                return;

            notification.IsRead = true;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}