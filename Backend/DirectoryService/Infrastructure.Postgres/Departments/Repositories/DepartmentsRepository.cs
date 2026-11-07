using Core.Departments.Interfaces;
using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class DepartmentsRepository : IDepartmentsRepository
    {
        private readonly AppDbContext _dbContext;

        public DepartmentsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddAsync(Department department, CancellationToken cancellationToken)
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            return department.Id;
        }

        public async Task<Guid> AddWithLocationsAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken)
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            foreach (var departmentLocation in departmentLocations)
            {
                await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
            }

            return department.Id;
        }

        public async Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            return await _dbContext.Departments.Where(p => p.Id == departmentId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistChildrenAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            return await _dbContext.Departments.Where(p => p.ParentId == departmentId).AnyAsync(cancellationToken);
        }

        public async Task RemoveAsync(Department department, CancellationToken cancellationToken)
        {
            _dbContext.Departments.Remove(department);
        }
    }
}
