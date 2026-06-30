namespace TaskFlow.Application.Features.Dashboard.Dtos
{
    public sealed class ProjectActivityDto
    {
        public Guid Id { get; init; }
        public string Action { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string ActorName { get; init; } = default!;
        public Guid? ProjectId { get; init; }
        public string? ProjectName { get; init; }
        public Guid? BoardId { get; init; }
        public string? BoardName { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}
