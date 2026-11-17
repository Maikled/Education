using Contracts.DTOs;
using Core.Locations.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Locations.Repositories
{
    internal class LocationReadRepository : ILocationReadRepository
    {
        private readonly IQueryable<Location> _queryLocations;

        public LocationReadRepository(AppDbContext dbContext)
        {
            _queryLocations = dbContext.Locations.AsQueryable().AsNoTracking();
        }

        public async Task<LocationDto?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
        {
            return await _queryLocations
                .Where(p => p.Id == locationId)
                .Select(p => new LocationDto(
                    p.Id,
                    p.Name.Value,
                    new AddressDto(p.Address.Country, p.Address.State, p.Address.City, p.Address.Street, p.Address.BuildingNumber),
                    p.CreatedAt,
                    p.UpdatedAt))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
