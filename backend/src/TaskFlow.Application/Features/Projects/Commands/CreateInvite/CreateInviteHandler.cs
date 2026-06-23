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

        public CreateInviteHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            CreateInviteCommand request,
            CancellationToken cancellationToken)
        {
            bool isAdmin = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == _currentUser.UserId &&
                    x.Role == ProjectMemberRole.Admin,
                    cancellationToken);

            Project? project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (project is null)
                throw new InvalidOperationException("Project not found");

            bool isOwner = project.OwnerId == _currentUser.UserId;

            if (!isAdmin && !isOwner)
                throw new UnauthorizedAccessException("You are not allowed to invite members.");

            bool alreadyMember = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == request.InvitedUserId,
                    cancellationToken) || project.OwnerId == request.InvitedUserId;

            if (alreadyMember)
                throw new InvalidOperationException("User is already a member of this project.");

            bool alreadyInvited = await _context.Invites
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.InvitedUserId == request.InvitedUserId &&
                    x.Status == InviteStatus.Pending,
                    cancellationToken);

            if (alreadyInvited)
                throw new InvalidOperationException("User already has a pending invite.");

            Invite invite = new()
            {
                ProjectId = request.ProjectId,
                InvitedUserId = request.InvitedUserId,
                InvitedByUserId = _currentUser.UserId,
                Role = request.Role,
                Status = InviteStatus.Pending
            };

            _context.Invites.Add(invite);

            Notification notification = new()
            {
                UserId = request.InvitedUserId,
                Type = NotificationType.ProjectInvite,
                Title = "Project invitation",
                Message = $"You've been invited to join \"{project.Name}\".",
                RelatedEntityId = invite.Id,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}