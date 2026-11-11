using Contracts.DTOs;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentReadRepository
    {
        public Task<DepartmentDto?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);
    }
}
