using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Auth;

namespace TaskFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            services.AddScoped<JwtTokenService>();
            services.AddScoped<PasswordHasher>();

            return services;
        }
    }
}
