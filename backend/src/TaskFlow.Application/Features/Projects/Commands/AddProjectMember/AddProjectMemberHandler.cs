using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.AddProjectMember
{
    public sealed class AddProjectMemberHandler
        : IRequestHandler<AddProjectMemberCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;

        public AddProjectMemberHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
        }

        public async Task Handle(
            AddProjectMemberCommand request,
            CancellationToken cancellationToken)
        {
            bool isAdmin = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == _currentUser.UserId &&
                    x.Role == ProjectMemberRole.Admin,
                    cancellationToken);

            if (!isAdmin)
                throw new UnauthorizedAccessException("You are not allowed to add members.");

            bool exists = await _context.ProjectMembers
                .AnyAsync(x =>
                        x.ProjectId == request.ProjectId &&
                        x.UserId == request.UserId,
                        cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("User already in project");
            }

            ProjectMember member = new()
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId,
                Role = request.Role
            };

            _context.ProjectMembers.Add(member);

            await _activityLogger.LogAsync(
                _currentUser.UserId, "MemberAdded", "Project",
                request.ProjectId, $"Added user {request.UserId} to project", cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
