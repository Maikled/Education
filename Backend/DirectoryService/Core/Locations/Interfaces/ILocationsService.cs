using Contracts.DTOs;

namespace Core.Locations.Interfaces
{
    public interface ILocationsService
    {
        public Task<Guid> CreateLocationAsync(CreateLocationDto dto, CancellationToken cancellationToken);
    }
}
