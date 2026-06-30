using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.DeclineInvite
{
    public sealed class DeclineInviteHandler
        : IRequestHandler<DeclineInviteCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public DeclineInviteHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            DeclineInviteCommand request,
            CancellationToken cancellationToken)
        {
            Invite? invite = await _context.Invites
                .FirstOrDefaultAsync(x => x.Id == request.InviteId, cancellationToken);

            if (invite is null)
                throw new InvalidOperationException("Invite not found.");

            if (invite.InvitedUserId != _currentUser.UserId)
                throw new UnauthorizedAccessException();

            if (invite.Status != InviteStatus.Pending)
                throw new InvalidOperationException("Invite is no longer pending.");

            invite.Status = InviteStatus.Declined;
            invite.RespondedAtUtc = DateTime.UtcNow;

            Notification? notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.RelatedEntityId == invite.Id &&
                    n.UserId == _currentUser.UserId,
                    cancellationToken);

            if (notification is not null)
            {
                notification.IsRead = true;
                notification.Title = "Invite declined";
                notification.Message = "You declined the invite.";
            }

            Project? project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == invite.ProjectId, cancellationToken);

            User? invitedUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == invite.InvitedUserId, cancellationToken);

            _context.Notifications.Add(new Notification
            {
                UserId = invite.InvitedByUserId,
                Type = NotificationType.InviteDeclined,
                Title = "Invite declined",
                Message = $"{invitedUser?.FirstName} {invitedUser?.LastName} declined your invite to \"{project?.Name}\".",
                RelatedEntityId = invite.Id,
                IsRead = false
            });

            await _activityLogger.LogAsync(
                _currentUser.UserId, "InviteDeclined", "Project",
                invite.ProjectId, $"Declined invite to \"{project?.Name}\"", project?.Id, project?.Name, null, null, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}