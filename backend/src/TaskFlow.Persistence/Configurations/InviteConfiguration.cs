using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class InviteConfiguration
        : IEntityTypeConfiguration<Invite>
    {
        public void Configure(
            EntityTypeBuilder<Invite> builder)
        {
            builder.HasOne(i => i.Project)
                .WithMany()
                .HasForeignKey(i => i.ProjectId);

            builder.HasOne(i => i.InvitedUser)
                .WithMany(u => u.ReceivedInvites)
                .HasForeignKey(i => i.InvitedUserId);

            builder.HasOne(i => i.InvitedBy)
                .WithMany(u => u.SentInvites)
                .HasForeignKey(i => i.InvitedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i =>
                new { i.ProjectId, i.InvitedUserId, i.Status });
        }
    }
}