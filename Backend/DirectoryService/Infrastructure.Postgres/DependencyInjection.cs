using Core.Locations.Interfaces;
using Infrastructure.Postgres.Locations.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructurePostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<ILocationsRepository, EfLocationsRepository>();
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            //services.AddSingleton<DapperContextFactory>((options) => new DapperContextFactory(connectionString, options.GetRequiredService<ILoggerFactory>()));
            //services.AddScoped<ILocationsRepository, DapperLocationsRepository>();

            return services;
        }
    }
}
