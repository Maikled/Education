using Core.Locations.Interfaces;
using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Infrastructure.Postgres.Locations.Repositories
{
    internal class LocationsRepository : ILocationsRepository
    {
        public Task AddAsync(Location location, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
