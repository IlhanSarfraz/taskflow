using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class NotificationConfiguration
        : IEntityTypeConfiguration<Notification>
    {
        public void Configure(
            EntityTypeBuilder<Notification> builder)
        {
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            builder.HasIndex(n =>
                new { n.UserId, n.IsRead });
        }
    }
}