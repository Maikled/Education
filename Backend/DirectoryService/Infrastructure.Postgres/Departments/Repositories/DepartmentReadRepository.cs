using Contracts;
using Contracts.DTOs;
using Core.Departments.GetDepartments.Enums;
using Core.Departments.Interfaces;
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

        public async Task<PagedResult<DepartmentListItemDto>> GetAllAsync(string? search, SortFields? sortFields, SortDirection sortDirection, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _queryDepartments;

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => EF.Functions.ILike(p.Name.Value, $"%{search}%"));

            if (sortFields.HasValue)
            {
                query = sortFields.Value switch
                {
                    SortFields.Name => OrderBySortDirection(query, p => p.Name.Value, sortDirection),
                    SortFields.Slug => OrderBySortDirection(query, p => p.Slug.Value, sortDirection),
                    SortFields.CreatedAt => OrderBySortDirection(query, p => p.CreatedAt, sortDirection),
                    SortFields.ParentId => OrderBySortDirection(query, p => p.ParentId, sortDirection),
                    _ => query
                };
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var results = await query.Select(p => new DepartmentListItemDto(p.Id, p.Name.Value, p.Slug.Value, p.Path.Value, p.CreatedAt)).ToListAsync(cancellationToken);

            return new PagedResult<DepartmentListItemDto>(results, totalCount, page, pageSize);
        }

        private static IQueryable<Department> OrderBySortDirection<TValue>(IQueryable<Department> query, Expression<Func<Department, TValue>> keySelector, SortDirection sortDirection)
        {
            return sortDirection == SortDirection.Ascending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);
        }
    }
}
