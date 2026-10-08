using Core.Departments.Interfaces;
using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class EfDepartmentsRepository : IDepartmentsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EfDepartmentsRepository> _logger;

        public EfDepartmentsRepository(AppDbContext dbContext, ILogger<EfDepartmentsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(Department department, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.Departments.AddAsync(department, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return department.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a department.");
                throw;
            }
        }

        public async Task<Guid> AddWithLocationsAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _dbContext.Departments.AddAsync(department, cancellationToken);

                foreach (var departmentLocation in departmentLocations)
                {
                    await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return department.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(ex, "An error occurred while adding a department with locations.");
                throw;
            }
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
