using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Persistence.Context
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry<BaseEntity>> entries = ChangeTracker
                .Entries<BaseEntity>();

            foreach (EntityEntry<BaseEntity> entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
