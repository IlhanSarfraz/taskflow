using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities
{
    public sealed class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public bool IsActive => RevokedAtUtc == null && DateTime.UtcNow < ExpiresAtUtc;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
