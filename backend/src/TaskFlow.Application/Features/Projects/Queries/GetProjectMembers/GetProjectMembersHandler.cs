using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.DTOs;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectMembers
{
    public sealed class GetProjectMembersHandler
        : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetProjectMembersHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ProjectMemberResponse>> Handle(
            GetProjectMembersQuery request,
            CancellationToken cancellationToken)
        {
            bool hasAccess = await _context.ProjectMembers
                .AnyAsync(x =>
                    x.ProjectId == request.ProjectId &&
                    x.UserId == _currentUser.UserId,
                    cancellationToken);

            if (!hasAccess)
                throw new UnauthorizedAccessException();

            return await _context.ProjectMembers
                .Where(x => x.ProjectId == request.ProjectId)
                .Select(x => new ProjectMemberResponse(
                    x.UserId,
                    x.User.FirstName + " " + x.User.LastName,
                    x.User.Email,
                    x.Role.ToString()
                ))
                .ToListAsync(cancellationToken);
        }
    }
}