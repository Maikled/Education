using Domain.Entities;

namespace Core.Locations.Interfaces
{
    public interface ILocationReadRepository
    {
        public Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);
    }
}
