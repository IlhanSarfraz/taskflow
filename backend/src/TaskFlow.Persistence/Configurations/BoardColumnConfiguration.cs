using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Configurations
{
    public sealed class BoardColumnConfiguration : IEntityTypeConfiguration<BoardColumn>
    {
        public void Configure(EntityTypeBuilder<BoardColumn> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Order)
                .IsRequired();

            builder.HasOne(x => x.Board)
                .WithMany(x => x.Columns)
                .HasForeignKey(x => x.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
