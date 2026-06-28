namespace TaskFlow.Application.Features.Dashboard.Dtos;

public sealed class DashboardOverviewDto
{
    public int AssignedCount { get; init; }
    public int DueTodayCount { get; init; }
    public int OverdueCount { get; init; }
    public int CompletedThisWeekCount { get; init; }
    public List<DueTaskDto> DueOrOverdueTasks { get; init; } = new();
}
