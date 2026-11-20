using Contracts;
using Contracts.DTOs;
using Core.Departments.GetDepartments.Enums;
using Core.Enums;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentReadRepository
    {
        public Task<DepartmentDto?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);
        public Task<PagedResult<DepartmentListItemDto>> GetDepartmentsAsync(string? search, DepartmentSortFields? sortFields, SortDirection sortDirection, int page, int pageSize, CancellationToken cancellationToken);
    }
}
