using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Projects.Commands.RemoveProjectMember
{
    public sealed class RemoveProjectMemberHandler
        : IRequestHandler<RemoveProjectMemberCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IActivityLogger _activityLogger;
        private readonly IProjectAuthorizationService _auth;

        public RemoveProjectMemberHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser,
            IActivityLogger activityLogger,
            IProjectAuthorizationService auth)
        {
            _context = context;
            _currentUser = currentUser;
            _activityLogger = activityLogger;
            _auth = auth;
        }

        public async Task Handle(
            RemoveProjectMemberCommand request,
            CancellationToken cancellationToken)
        {
            await _auth
                .EnsureProjectManagerAsync(request.ProjectId, cancellationToken);

            ProjectMember? member = await _context.ProjectMembers
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == request.UserId,
                    cancellationToken);

            if (member == null)
                return;

            _context.ProjectMembers.Remove(member);

            await _activityLogger.LogAsync(
                _currentUser.UserId, "MemberRemoved", "Project",
                request.ProjectId, $"Removed user {request.UserId} from project", cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
