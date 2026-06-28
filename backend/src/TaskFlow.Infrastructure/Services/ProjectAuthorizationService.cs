using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Infrastructure.Services;

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
        {
            bool isOwner = await _context.Projects
                .AnyAsync(x =>
                    x.Id == projectId &&
                    x.OwnerId == _currentUser.UserId,
                    cancellationToken);

            if (!isOwner)
                throw new UnauthorizedAccessException(
                    "You are not a project member.");
        }
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

    public async Task EnsureProjectManagerAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        bool isAdmin = await _context.ProjectMembers
            .AnyAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == _currentUser.UserId &&
                x.Role == ProjectMemberRole.Admin,
                cancellationToken);

        bool isOwner = await _context.Projects
            .AnyAsync(x =>
                x.Id == projectId &&
                x.OwnerId == _currentUser.UserId,
                cancellationToken);

        if (!isAdmin && !isOwner)
        {
            throw new UnauthorizedAccessException(
                "Admin or owner access required.");
        }
    }

    public async Task EnsureTaskMemberAsync(Guid taskId, CancellationToken cancellationToken)
    {
        bool isMember = await _context.ProjectMembers
            .AnyAsync(x =>
                x.Project.Tasks.Any(t => t.Id == taskId) &&
                x.UserId == _currentUser.UserId,
                cancellationToken);

        if (!isMember)
        {
            bool isOwner = await _context.Projects
                .AnyAsync(x =>
                    x.Tasks.Any(t => t.Id == taskId) &&
                    x.OwnerId == _currentUser.UserId,
                    cancellationToken);

            if (!isOwner)
                throw new UnauthorizedAccessException(
                    "You are not a project member");
        }
    }

    public async Task<List<Guid>> GetAccessibleProjectIdsAsync(CancellationToken cancellationToken = default)
    {
        List<Guid> memberProjectIds = await _context.ProjectMembers
            .Where(x => x.UserId == _currentUser.UserId)
            .Select(x => x.ProjectId)
            .ToListAsync(cancellationToken);

        List<Guid> ownedProjectIds = await _context.Projects
            .Where(x => x.OwnerId == _currentUser.UserId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return memberProjectIds.Union(ownedProjectIds).ToList();
    }
}