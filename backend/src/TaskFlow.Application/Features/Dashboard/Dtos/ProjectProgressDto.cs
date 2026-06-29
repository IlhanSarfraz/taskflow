public sealed class ProjectProgressDto
{
    public Guid ProjectId { get; init; }
    public string ProjectName { get; init; } = default!;
    public int TotalTaskCount { get; init; }
    public int CompletedTaskCount { get; init; }
    public int ProgressPercent { get; init; }
}