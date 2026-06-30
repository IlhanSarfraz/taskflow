using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Features.Dashboard.Dtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Features.Dashboard.Queries.GetDashboardOverview;

public sealed class GetDashboardOverviewHandler
    : IRequestHandler<GetDashboardOverviewQuery, DashboardOverviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorizationService _authorization;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardOverviewHandler(
        IApplicationDbContext context,
        IProjectAuthorizationService authorization,
        ICurrentUserService currentUser)
    {
        _context = context;
        _authorization = authorization;
        _currentUser = currentUser;
    }

    public async Task<DashboardOverviewDto> Handle(
        GetDashboardOverviewQuery request,
        CancellationToken cancellationToken)
    {
        List<Guid> projectIds = await _authorization
            .GetAccessibleProjectIdsAsync(cancellationToken);

        DateTime today = DateTime.UtcNow.Date;
        DateTime weekAgo = today.AddDays(-7);

        IQueryable<TaskItem> myOpenTasksQuery = _context.Tasks
            .Where(t => projectIds.Contains(t.ProjectId))
            .Where(t => t.Assignments.Any(a => a.UserId == _currentUser.UserId))
            .Where(t => !t.BoardColumn.IsDoneColumn);

        int assignedCount = await myOpenTasksQuery
            .CountAsync(cancellationToken);

        int dueTodayCount = await myOpenTasksQuery
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == today)
            .CountAsync(cancellationToken);

        int overdueCount = await myOpenTasksQuery
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date < today)
            .CountAsync(cancellationToken);

        int completedThisWeekCount = await _context.Tasks
            .Where(t => projectIds.Contains(t.ProjectId))
            .Where(t => t.Assignments.Any(a => a.UserId == _currentUser.UserId))
            .Where(t => t.BoardColumn.IsDoneColumn)
            .Where(t => t.CompletedAtUtc >= weekAgo)
            .CountAsync(cancellationToken);

        List<DueTaskDto> dueOrOverdueTasks = await myOpenTasksQuery
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date <= today)
            .OrderBy(t => t.DueDate)
            .Select(t => new DueTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Priority = (int)t.Priority,
                DueDate = t.DueDate!.Value,
                IsOverdue = t.DueDate!.Value.Date < today,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                BoardId = t.BoardColumn.BoardId,
                BoardName = t.BoardColumn.Board.Name
            })
            .ToListAsync(cancellationToken);

        List<ProjectProgressDto> projectProgress = await _context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .Select(p => new ProjectProgressDto
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                TotalTaskCount = p.Tasks.Count,
                CompletedTaskCount = p.Tasks.Count(t => t.BoardColumn.IsDoneColumn),
                ProgressPercent = p.Tasks.Count == 0
                    ? 0
                    : (int)Math.Round(
                        p.Tasks.Count(t => t.BoardColumn.IsDoneColumn) * 100.0 / p.Tasks.Count)
            })
            .ToListAsync(cancellationToken);

        // Cross-project activity feed: all activity rows belonging to projects
        // the current user can access, ordered newest first, capped at 30.
        // ActorName resolved via join — "You" substituted on the frontend
        // when ActorUserId matches the logged-in user.
        List<ProjectActivityDto> activity = await _context.ActivityLogs
            .Where(a => a.ProjectId != null && projectIds.Contains(a.ProjectId.Value))
            .OrderByDescending(a => a.CreatedAtUtc)
            .Take(30)
            .Select(a => new ProjectActivityDto
            {
                Id = a.Id,
                Action = a.Action,
                Description = a.Description,
                ActorName = a.User.FirstName + " " + a.User.LastName,
                ProjectId = a.ProjectId,
                ProjectName = a.ProjectName,
                BoardId = a.BoardId,
                BoardName = a.BoardName,
                CreatedAtUtc = a.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new DashboardOverviewDto
        {
            AssignedCount = assignedCount,
            DueTodayCount = dueTodayCount,
            OverdueCount = overdueCount,
            CompletedThisWeekCount = completedThisWeekCount,
            DueOrOverdueTasks = dueOrOverdueTasks,
            Projects = projectProgress,
            Activity = activity
        };
    }
}