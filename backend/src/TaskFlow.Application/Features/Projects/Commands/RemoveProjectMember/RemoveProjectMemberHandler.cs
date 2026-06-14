using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands.RemoveProjectMember
{
    public sealed class RemoveProjectMemberHandler
        : IRequestHandler<RemoveProjectMemberCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public RemoveProjectMemberHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(
            RemoveProjectMemberCommand request,
            CancellationToken cancellationToken)
        {
            bool isAdmin = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == _currentUser.UserId &&
                    x.Role == ProjectMemberRole.Admin,
                    cancellationToken);

            if (!isAdmin)
                throw new UnauthorizedAccessException("You are not allowed to remove members.");

            ProjectMember? member = await _context.ProjectMembers
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == request.UserId,
                    cancellationToken);

            if (member == null)
                return;

            _context.ProjectMembers.Remove(member);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
