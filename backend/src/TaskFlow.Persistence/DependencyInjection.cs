using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Persistence.Context;

namespace TaskFlow.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IApplicationDbContext>(
                provider => provider.GetRequiredService<AppDbContext>());

            return services;
        }
    }
}
