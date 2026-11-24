using Contracts;
using Contracts.DTOs;
using Core.Departments.GetDepartments.Enums;
using Core.Departments.Interfaces;
using Core.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public async Task<PagedResult<DepartmentListItemDto>> GetDepartmentsAsync(string? search, DepartmentSortFields? sortFields, SortDirection sortDirection, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _queryDepartments;

            if (!string.IsNullOrWhiteSpace(search))
                // Name хранится через value converter: обращение к .Value не транслируется в SQL,
                // поэтому приводим свойство к типу колонки (string)
                query = query.Where(p => EF.Functions.ILike((string)(object)p.Name, $"%{search}%"));

            Expression<Func<Department, object>> expression = sortFields.HasValue ? sortFields.Value switch
            {
                DepartmentSortFields.Name => p => p.Name,
                DepartmentSortFields.Slug => p => p.Slug,
                DepartmentSortFields.CreatedAt => p => p.CreatedAt,
                DepartmentSortFields.ParentId => p => p.ParentId,
                _ => p => p.CreatedAt
            } : p => p.CreatedAt;

            if (sortDirection == SortDirection.Ascending)
            {
                query = query.OrderBy(expression);
            }
            else
            {
                query = query.OrderByDescending(expression);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var results = await query.Select(p => new DepartmentListItemDto(p.Id, p.Name.Value, p.Slug.Value, p.Path.Value, p.CreatedAt)).ToListAsync(cancellationToken);

            return new PagedResult<DepartmentListItemDto>(results, totalCount, page, pageSize);
        }
    }
}
