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
        List<Guid> projectIds = await _authorization.GetAccessibleProjectIdsAsync(cancellationToken);

        DateTime today = DateTime.UtcNow.Date;
        DateTime weekAgo = today.AddDays(-7);

        // Base set: tasks in my projects, assigned to me, not in a done column.
        IQueryable<TaskItem> myOpenTasksQuery = _context.Tasks
            .Where(t => projectIds.Contains(t.ProjectId))
            .Where(t => t.Assignments.Any(a => a.UserId == _currentUser.UserId))
            .Where(t => !t.BoardColumn.IsDoneColumn);

        int assignedCount = await myOpenTasksQuery.CountAsync(cancellationToken);

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

        return new DashboardOverviewDto
        {
            AssignedCount = assignedCount,
            DueTodayCount = dueTodayCount,
            OverdueCount = overdueCount,
            CompletedThisWeekCount = completedThisWeekCount,
            DueOrOverdueTasks = dueOrOverdueTasks
        };
    }
}