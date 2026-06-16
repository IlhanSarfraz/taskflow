using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Member;

        public bool IsActive { get; set; } = true;

        public ICollection<RefreshToken> RefreshTokens { get; set; }
            = new List<RefreshToken>();

        public ICollection<Project> Projects { get; set; }
            = new List<Project>();

        public ICollection<ProjectMember> ProjectMemberships { get; set; }
            = new List<ProjectMember>();

        public ICollection<TaskComment> Comments { get; set; }
            = new List<TaskComment>();
    }
}
