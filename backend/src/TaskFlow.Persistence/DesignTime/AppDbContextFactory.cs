using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TaskFlow.Persistence.Context;

namespace TaskFlow.Persistence.DesignTime
{
    public class AppDbContextFactory :
        IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<AppDbContext> optionsBuilder =
                new();

            optionsBuilder.UseSqlite("Data Source=taskflow.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
