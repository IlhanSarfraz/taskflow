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

    public async Task EnsureMemberAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        bool isMember = await _context.ProjectMembers
            .AnyAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == _currentUser.UserId,
                cancellationToken);

        if (!isMember)
            throw new UnauthorizedAccessException(
                "You are not a project member.");
    }

    public async Task EnsureAdminAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        bool isAdmin = await _context.ProjectMembers
            .AnyAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == _currentUser.UserId &&
                x.Role == ProjectMemberRole.Admin,
                cancellationToken);

        if (!isAdmin)
            throw new UnauthorizedAccessException(
                "Admin access required.");
    }

    public async Task EnsureTaskMemberAsync(Guid taskId, CancellationToken cancellationToken)
    {
        bool isMember = await _context.ProjectMembers
            .AnyAsync(x =>
                x.Project.Tasks.Any(t => t.Id == taskId) &&
                x.UserId == _currentUser.UserId,
                cancellationToken);

        if (!isMember)
            throw new UnauthorizedAccessException(
                "You are not a project member");
    }
}