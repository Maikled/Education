using Domain.Entities;
using Domain.Entities.AssociativeEntities;

namespace Core.Departments.Interfaces
{
    public interface IWriteDepartmentsRepository
    {
        public Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);
        public Task<Guid> AddWithLocationsAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken);
        public Task RemoveAsync(Department department, CancellationToken cancellationToken);
    }
}
