using Contracts;
using Contracts.DTOs;
using Core.Departments.GetDepartments.Enums;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentReadRepository
    {
        public Task<DepartmentDto?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);
        public Task<PagedResult<DepartmentListItemDto>> GetAllAsync(string? search, SortFields? sortFields, SortDirection sortDirection, int page, int pageSize, CancellationToken cancellationToken);
    }
}
