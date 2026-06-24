using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public class ActivityLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Action { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty;

        public Guid EntityId { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}