using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Users.DTOs;

namespace TaskFlow.Application.Features.Users.Queries.GetUserActivity
{
    public sealed class GetUserActivityHandler
        : IRequestHandler<GetUserActivityQuery, List<ActivityLogResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetUserActivityHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ActivityLogResponse>> Handle(
            GetUserActivityQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.ActivityLogs
                .AsNoTracking()
                .Where(x => x.UserId == _currentUser.UserId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ActivityLogResponse(
                    x.Id,
                    x.Action,
                    x.EntityType,
                    x.EntityId,
                    x.Description,
                    x.CreatedAtUtc))
                .ToListAsync(cancellationToken);
        }
    }
}