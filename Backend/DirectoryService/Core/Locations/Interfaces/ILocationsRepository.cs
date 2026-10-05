using Domain.Entities;
using Domain.Entities.ValueObjects;

namespace Core.Locations.Interfaces
{
    public interface ILocationsRepository
    {
        public Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);
        public Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken);
        public Task<bool> ExistById(Guid locationId, CancellationToken cancellationToken);
        public Task<bool> ExistAll(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
    }
}
