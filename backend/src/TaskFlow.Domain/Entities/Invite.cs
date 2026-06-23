using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public sealed class Invite : BaseEntity
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public Guid InvitedUserId { get; set; }

        public User InvitedUser { get; set; } = null!;

        public Guid InvitedByUserId { get; set; }

        public User InvitedBy { get; set; } = null!;

        public ProjectMemberRole Role { get; set; }
            = ProjectMemberRole.Member;

        public InviteStatus Status { get; set; }
            = InviteStatus.Pending;

        public DateTime? RespondedAtUtc { get; set; }
    }
}