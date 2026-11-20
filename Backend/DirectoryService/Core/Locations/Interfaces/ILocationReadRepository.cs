using Contracts;
using Contracts.DTOs;
using Core.Enums;
using Core.Locations.GetLocations.Enums;

namespace Core.Locations.Interfaces
{
    public interface ILocationReadRepository
    {
        public Task<LocationDto?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);
        public Task<IEnumerable<TopLocationDto>> GetTopLocationsAsync(int limit, CancellationToken cancellationToken);
        public Task<PagedResult<LocationListItemDto>> GetLocationsAsync(string? search, int? minDepartments, LocationSortFields? sortField, SortDirection sortDirection, int page, int pageSize, CancellationToken cancellationToken);
    }
}
