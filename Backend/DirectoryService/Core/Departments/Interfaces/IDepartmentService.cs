using Contracts.DTOs;
using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentService
    {
        public Task<Result<Guid, AppError>> CreateDepartmentAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
        public Task<UnitResult<AppError>> UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentDto dto, CancellationToken cancellationToken);
        public Task<UnitResult<AppError>> AddLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
        public Task<UnitResult<AppError>> RemoveLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    }
}
