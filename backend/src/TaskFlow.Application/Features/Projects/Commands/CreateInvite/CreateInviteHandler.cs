using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.CreateInvite
{
    public sealed class CreateInviteHandler
        : IRequestHandler<CreateInviteCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public CreateInviteHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            CreateInviteCommand request,
            CancellationToken cancellationToken)
        {
            Project? project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == request.ProjectId,
                    cancellationToken);

            if (project is null)
                throw new KeyNotFoundException("Project not found.");

            bool isAdmin = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == _currentUser.UserId &&
                    x.Role == ProjectMemberRole.Admin,
                    cancellationToken);

            bool isOwner = project.OwnerId == _currentUser.UserId;

            if (!isAdmin && !isOwner)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to invite members.");
            }

            User? invitedUser = await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Email == request.InvitedEmail,
                    cancellationToken);

            if (invitedUser is null)
            {
                throw new KeyNotFoundException(
                    $"No user found with email '{request.InvitedEmail}'.");
            }

            bool alreadyMember = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == invitedUser.Id,
                    cancellationToken)
                || project.OwnerId == invitedUser.Id;

            if (alreadyMember)
            {
                throw new InvalidOperationException(
                    "User is already a member of this project.");
            }

            bool alreadyInvited = await _context.Invites
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.InvitedUserId == invitedUser.Id &&
                    x.Status == InviteStatus.Pending,
                    cancellationToken);

            if (alreadyInvited)
            {
                throw new InvalidOperationException(
                    "User already has a pending invite.");
            }

            Invite invite = new()
            {
                ProjectId = request.ProjectId,
                InvitedUserId = invitedUser.Id,
                InvitedByUserId = _currentUser.UserId,
                Role = request.Role,
                Status = InviteStatus.Pending
            };

            _context.Invites.Add(invite);

            Notification notification = new()
            {
                UserId = invitedUser.Id,
                Type = NotificationType.ProjectInvite,
                Title = "Project invitation",
                Message = $"You've been invited to join \"{project.Name}\".",
                RelatedEntityId = invite.Id,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _activityLogger.LogAsync(
                _currentUser.UserId,
                "InviteSent",
                "Project",
                request.ProjectId,
                $"Invited {invitedUser.Email} to \"{project.Name}\"",
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}