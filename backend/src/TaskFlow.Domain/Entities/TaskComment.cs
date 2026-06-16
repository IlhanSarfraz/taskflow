using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public sealed class TaskComment : BaseEntity
    {
        public Guid TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
    }
}