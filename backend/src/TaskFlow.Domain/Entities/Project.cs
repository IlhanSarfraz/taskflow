using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!;

        public ICollection<Board> Boards { get; set; }
            = new List<Board>();

        public ICollection<TaskItem> Tasks { get; set; }
            = new List<TaskItem>();

        public ICollection<ProjectMember> Members { get; set; }
            = new List<ProjectMember>();
    }
}
