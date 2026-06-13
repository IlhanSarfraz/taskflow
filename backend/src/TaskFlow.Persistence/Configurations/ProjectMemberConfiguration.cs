using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class ProjectMemberConfiguration
        : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(
            EntityTypeBuilder<ProjectMember> builder)
        {
            builder.HasOne(pm => pm.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId);

            builder.HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId);

            builder.HasIndex(pm =>
                new { pm.ProjectId, pm.UserId })
                .IsUnique();
        }
    }
}