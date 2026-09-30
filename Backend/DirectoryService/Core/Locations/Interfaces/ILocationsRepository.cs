using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Core.Locations.Interfaces
{
    public interface ILocationsRepository
    {
        public Task AddAsync(Location location, CancellationToken cancellationToken);
        public Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken);
    }
}
