using Core.Departments.Interfaces;
using Core.Interfaces;
using Core.Locations.Interfaces;
using Core.Positions.Interfaces;
using Infrastructure.Postgres.Departments.Repositories;
using Infrastructure.Postgres.Locations.Repositories;
using Infrastructure.Postgres.Positions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructurePostgresServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<ITransactionManager, TransactionManager>();

            services.AddScoped<ILocationsRepository, LocationsRepository>();
            services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
            services.AddScoped<IDepartmentLocationsRepository, DepartmentLocationsRepository>();
            services.AddScoped<IDepartmentPositionsRepository, DepartmentPositionsRepository>();
            services.AddScoped<IPositionsRepository, PositionsRepository>();

            services.AddScoped<IDepartmentReadRepository, DepartmentReadRepository>();
            services.AddScoped<ILocationReadRepository, LocationReadRepository>();

            return services;
        }
    }
}
