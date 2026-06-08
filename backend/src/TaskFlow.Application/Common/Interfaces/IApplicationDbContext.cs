using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Project> Projects { get; }
        DbSet<Board> Boards { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
