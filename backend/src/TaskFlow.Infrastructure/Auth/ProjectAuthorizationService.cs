using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Infrastructure.Auth;

public sealed class ProjectAuthorizationService : IProjectAuthorizationService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ProjectAuthorizationService(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task EnsureMemberAsync(Guid projectId, CancellationToken ct = default)
    {
        bool isMember = await _context.ProjectMembers
            .AnyAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == _currentUser.UserId,
                ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a project member.");
    }

    public async Task EnsureAdminAsync(Guid projectId, CancellationToken ct = default)
    {
        bool isAdmin = await _context.ProjectMembers
            .AnyAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == _currentUser.UserId &&
                x.Role == ProjectMemberRole.Admin,
                ct);

        if (!isAdmin)
            throw new UnauthorizedAccessException("Admin access required.");
    }
}