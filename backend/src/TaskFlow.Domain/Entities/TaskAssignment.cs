namespace TaskFlow.Domain.Entities;

public sealed class TaskAssignment
{
    public Guid TaskId { get; set; }

    public TaskItem Task { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
