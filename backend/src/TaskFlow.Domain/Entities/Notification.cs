using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public sealed class Notification : BaseEntity
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public Guid? RelatedEntityId { get; set; }

        public bool IsRead { get; set; } = false;
    }
}