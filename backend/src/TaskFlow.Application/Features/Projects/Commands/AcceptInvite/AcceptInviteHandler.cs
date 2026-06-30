using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.AcceptInvite
{
    public sealed class AcceptInviteHandler
        : IRequestHandler<AcceptInviteCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public AcceptInviteHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            AcceptInviteCommand request,
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

            invite.Status = InviteStatus.Accepted;
            invite.RespondedAtUtc = DateTime.UtcNow;

            ProjectMember member = new()
            {
                ProjectId = invite.ProjectId,
                UserId = invite.InvitedUserId,
                Role = invite.Role
            };

            _context.ProjectMembers.Add(member);

            Notification? originalNotification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.RelatedEntityId == invite.Id &&
                    n.UserId == _currentUser.UserId,
                    cancellationToken);

            if (originalNotification is not null)
            {
                originalNotification.IsRead = true;
                originalNotification.Title = "Invite accepted";
                originalNotification.Message = "You accepted the invite to join the project.";
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
                Type = NotificationType.InviteAccepted,
                Title = "Invite accepted",
                Message = $"{invitedUser?.FirstName} {invitedUser?.LastName} accepted your invite to \"{project?.Name}\".",
                RelatedEntityId = invite.Id,
                IsRead = false
            });

            await _activityLogger.LogAsync(
                _currentUser.UserId, "InviteAccepted", "Project",
                invite.ProjectId, $"Joined project \"{project?.Name}\"", invite.ProjectId, project?.Name, null, null, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}