using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Projects.DTOs;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Queries.GetMyInvites
{
    public sealed class GetMyInvitesHandler
        : IRequestHandler<GetMyInvitesQuery, List<InviteResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetMyInvitesHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<InviteResponse>> Handle(
            GetMyInvitesQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Invites
                .AsNoTracking()
                .Where(i =>
                    i.InvitedUserId == _currentUser.UserId &&
                    i.Status == InviteStatus.Pending)
                .OrderByDescending(i => i.CreatedAtUtc)
                .Select(i => new InviteResponse(
                    i.Id,
                    i.ProjectId,
                    i.Project.Name,
                    i.InvitedBy.FirstName + " " + i.InvitedBy.LastName,
                    i.Role.ToString(),
                    i.CreatedAtUtc))
                .ToListAsync(cancellationToken);
        }
    }
}