using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Constants;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Users.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Users.Queries.GetUserProfile
{
    public sealed class GetUserProfileHandler
        : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetUserProfileHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<UserProfileResponse> Handle(
            GetUserProfileQuery request,
            CancellationToken cancellationToken)
        {
            User user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == _currentUser.UserId,
                    cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            int tasksAssigned = await _context.Tasks
                .CountAsync(
                    x => x.Assignments.Any(a => a.UserId == _currentUser.UserId),
                    cancellationToken);

            int tasksCompleted = await _context.Tasks
                .CountAsync(
                    x => x.Assignments.Any(a => a.UserId == _currentUser.UserId) &&
                         x.BoardColumn.Name == BoardColumnNames.Done,
                    cancellationToken);

            int tasksInProgress = await _context.Tasks
                .CountAsync(
                    x => x.Assignments.Any(a => a.UserId == _currentUser.UserId) &&
                         x.BoardColumn.Name == BoardColumnNames.InProgress,
                    cancellationToken);

            int projectCount = await _context.ProjectMembers
                .CountAsync(
                    x => x.UserId == _currentUser.UserId,
                    cancellationToken);

            return new UserProfileResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.CreatedAtUtc,
                tasksAssigned,
                tasksCompleted,
                tasksInProgress,
                projectCount);
        }
    }
}
