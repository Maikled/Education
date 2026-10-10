using Domain.Entities;
using Domain.Entities.AssociativeEntities;

namespace Core.Departments.Interfaces
{
    public interface IDepartmentsRepository
    {
        public Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);
        public Task<Guid> AddWithLocationsAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken);
        public Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);
        public Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
