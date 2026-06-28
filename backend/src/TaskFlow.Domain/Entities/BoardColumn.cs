using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities;

public sealed class BoardColumn : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public Guid BoardId { get; set; }

    public bool IsDoneColumn { get; set; }

    public Board Board { get; set; } = null!;

    public ICollection<TaskItem> Tasks { get; set; }
        = new List<TaskItem>();
}