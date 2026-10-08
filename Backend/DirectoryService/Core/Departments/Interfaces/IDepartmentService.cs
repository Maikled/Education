using Contracts.DTOs;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Guid> CreateDepartmentAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
        public Task UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentDto dto, CancellationToken cancellationToken);
        public Task AddLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
        public Task RemoveLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    }
}
