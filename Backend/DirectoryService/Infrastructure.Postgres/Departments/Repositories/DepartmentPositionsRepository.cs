using Core.Departments.Interfaces;
using Domain.Entities.AssociativeEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class DepartmentPositionsRepository : IDepartmentPositionsRepository
    {
        private readonly AppDbContext _dbContext;

        public DepartmentPositionsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(DepartmentPosition departmentPosition, CancellationToken cancellationToken)
        {
            await _dbContext.DepartmentPositions.AddAsync(departmentPosition, cancellationToken);
        }

        public async Task<DepartmentPosition?> GetAsync(Guid departmentId, Guid positionId, CancellationToken cancellationToken)
        {
            return await _dbContext.DepartmentPositions.Where(p => p.DepartmentId == departmentId && p.PositionId == positionId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistByPositionIdAsync(Guid positionId, CancellationToken cancellationToken)
        {
            return await _dbContext.DepartmentPositions.Where(p => p.PositionId == positionId).AnyAsync(cancellationToken);
        }

        public async Task RemoveAsync(DepartmentPosition departmentPosition, CancellationToken cancellationToken)
        {
            _dbContext.DepartmentPositions.Remove(departmentPosition);
        }

        public async Task RemoveByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            var departmentPositions = await _dbContext.DepartmentPositions.Where(p => p.DepartmentId == departmentId).ToListAsync(cancellationToken);

            _dbContext.DepartmentPositions.RemoveRange(departmentPositions);
        }
    }
}
