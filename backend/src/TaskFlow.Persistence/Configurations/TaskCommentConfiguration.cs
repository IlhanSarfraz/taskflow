using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class TaskCommentConfiguration
        : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(
            EntityTypeBuilder<TaskComment> builder)
        {
            builder.Property(x => x.Content)
                .HasMaxLength(2000)
                .IsRequired();

            builder.HasOne(x => x.Task)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TaskId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId);
        }
    }
}
