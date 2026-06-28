namespace TaskFlow.Application.Features.Dashboard.Dtos
{
    public sealed class DueTaskDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = default!;
        public int Priority { get; init; }
        public DateTime DueDate { get; init; }
        public bool IsOverdue { get; init; }
        public Guid ProjectId { get; init; }
        public string ProjectName { get; init; } = default!;
        public Guid BoardId { get; init; }
        public string BoardName { get; init; } = default!;
    }
}
