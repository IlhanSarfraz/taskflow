using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public sealed class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskPriority Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public Guid BoardColumnId { get; set; }

        public BoardColumn BoardColumn { get; set; } = null!;
        public Guid? AssigneeId { get; set; }
    }
}
