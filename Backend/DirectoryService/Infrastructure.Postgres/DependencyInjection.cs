using Core.Departments.Interfaces;
using Core.Locations.Interfaces;
using Infrastructure.Postgres.Departments.Repositories;
using Infrastructure.Postgres.Locations.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructurePostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<ILocationsRepository, EfLocationsRepository>();
            services.AddScoped<IDepartmentsRepository, EfDepartmentsRepository>();
            services.AddScoped<IDepartmentLocationsRepository, EfDepartmentLocationsRepository>();

            //services.AddSingleton<DapperContextFactory>((options) => new DapperContextFactory(connectionString, options.GetRequiredService<ILoggerFactory>()));
            //services.AddScoped<ILocationsRepository, DapperLocationsRepository>();

            return services;
        }
    }
}
