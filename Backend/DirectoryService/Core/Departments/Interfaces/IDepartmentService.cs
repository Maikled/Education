using Contracts.DTOs;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Guid> CreateDepartmentAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
    }
}
