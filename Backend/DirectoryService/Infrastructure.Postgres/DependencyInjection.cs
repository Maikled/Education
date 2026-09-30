using Core.Locations.Interfaces;
using Infrastructure.Postgres.Locations.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Postgres
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructurePostgresServices(this IServiceCollection services)
        {
            services.AddScoped<ILocationsRepository, LocationsRepository>();

            return services;
        }
    }
}
