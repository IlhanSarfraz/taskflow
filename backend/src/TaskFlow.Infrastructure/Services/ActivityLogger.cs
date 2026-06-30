using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Services;

public sealed class ActivityLogger : IActivityLogger
{
    private readonly IApplicationDbContext _context;

    public ActivityLogger(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        string description,
        Guid? projectId,
        string? projectName,
        Guid? boardId,
        string? boardName,
        CancellationToken cancellationToken = default)
    {
        _context.ActivityLogs.Add(new ActivityLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            ProjectId = projectId,
            ProjectName = projectName,
            BoardId = boardId,
            BoardName = boardName
        });
    }
}