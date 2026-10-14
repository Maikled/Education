using Core.Locations.Interfaces;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Locations.Repositories
{
    internal class EfLocationsRepository : ILocationsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EfLocationsRepository> _logger;

        public EfLocationsRepository(AppDbContext dbContext, ILogger<EfLocationsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Locations.AddAsync(location, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return location.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a location.");
                throw;
            }
        }

        public async Task<bool> ExistWithNameAsync(Name name, CancellationToken cancellationToken)
        {
            return await _dbContext.Locations.Where(p => p.Name == name).AnyAsync(cancellationToken);
        }

        public async Task<bool> ExistById(Guid locationId, CancellationToken cancellationToken)
        {
            return await _dbContext.Locations.Where(p => p.Id == locationId).AnyAsync(cancellationToken);
        }

        public async Task<bool> ExistAll(IEnumerable<Guid> locationIds, CancellationToken cancellationToken)
        {
            var distinctLocationIds = locationIds.Distinct().ToList();

            var countLocations = await _dbContext.Locations.CountAsync(p => locationIds.Contains(p.Id), cancellationToken);

            return distinctLocationIds.Count == countLocations;
        }

        public async Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
        {
            return await _dbContext.Locations.Where(p => p.Id == locationId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
