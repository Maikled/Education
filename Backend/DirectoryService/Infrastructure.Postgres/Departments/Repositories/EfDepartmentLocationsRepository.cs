using Core.Departments.Interfaces;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class EfDepartmentLocationsRepository : IDepartmentLocationsRepository
    {
        private readonly AppDbContext _dbContext;

        public EfDepartmentLocationsRepository(AppDbContext dbContext)
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

        public async Task RemoveAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
        {
            _dbContext.DepartmentLocations.Remove(departmentLocation);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
