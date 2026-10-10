using Core.Departments.Interfaces;
using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class EfDepartmentsRepository : IDepartmentsRepository
    {
        private readonly AppDbContext _dbContext;

        public EfDepartmentsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddAsync(Department department, CancellationToken cancellationToken)
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return department.Id;
        }

        public async Task<Guid> AddWithLocationsAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.Departments.AddAsync(department, cancellationToken);

            foreach (var departmentLocation in departmentLocations)
            {
                await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return department.Id;
        }

        public async Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            return await _dbContext.Departments.Where(p => p.Id == departmentId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
