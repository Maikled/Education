using Contracts.DTOs;
using Core.Departments.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Departments.Repositories
{
    internal class DepartmentReadRepository : IDepartmentReadRepository
    {
        private readonly IQueryable<Department> _queryDepartments;

        public DepartmentReadRepository(AppDbContext dbContext)
        {
            _queryDepartments = dbContext.Departments.AsQueryable().AsNoTracking();
        }

        public async Task<DepartmentDto?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            return await _queryDepartments
                .Where(p => p.Id == departmentId)
                .Select(p => new DepartmentDto(p.Id, p.Name.Value, p.Slug.Value, p.Path.Value, p.ParentId, p.CreatedAt, p.UpdatedAt))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
