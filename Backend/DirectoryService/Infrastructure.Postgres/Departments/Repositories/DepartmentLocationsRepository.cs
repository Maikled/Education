using Core.Departments.Interfaces;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class DepartmentLocationsRepository : IDepartmentLocationsRepository
    {
        private readonly AppDbContext _dbContext;

        public DepartmentLocationsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
        {
            await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
        }

        public async Task<DepartmentLocation?> GetAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            return await _dbContext.DepartmentLocations.Where(p => p.DepartmentId == departmentId && p.LocationId == locationId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistByLocationIdAsync(Guid locationId, CancellationToken cancellationToken)
        {
            return await _dbContext.DepartmentLocations.Where(p => p.LocationId == locationId).AnyAsync(cancellationToken);
        }

        public async Task RemoveAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
        {
            _dbContext.DepartmentLocations.Remove(departmentLocation);
        }

        public async Task RemoveByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            var departmentLocations = await _dbContext.DepartmentLocations.Where(p => p.DepartmentId == departmentId).ToListAsync(cancellationToken);

            _dbContext.DepartmentLocations.RemoveRange(departmentLocations);
        }
    }
}
