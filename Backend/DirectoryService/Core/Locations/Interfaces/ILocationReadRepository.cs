using Contracts.DTOs;

namespace Core.Locations.Interfaces
{
    public interface ILocationReadRepository
    {
        public Task<LocationDto?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);
    }
}
