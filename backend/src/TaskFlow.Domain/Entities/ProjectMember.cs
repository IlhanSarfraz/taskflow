using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public sealed class ProjectMember : BaseEntity
    {
        public Guid ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public ProjectMemberRole Role { get; set; }
            = ProjectMemberRole.Member;
    }
}