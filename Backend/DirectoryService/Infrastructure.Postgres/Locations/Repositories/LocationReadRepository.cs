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

        public async Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
        {
            return await _queryLocations.Where(p => p.Id == locationId).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
