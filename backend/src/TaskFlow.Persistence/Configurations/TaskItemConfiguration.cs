using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class TaskItemConfiguration
        : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.Property(x => x.Title)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Priority)
                .IsRequired();

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId);

            builder.HasOne(x => x.BoardColumn)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.BoardColumnId);

            builder.Property(x => x.Order)
                .IsRequired();
        }
    }
}
