using Core.Interfaces;

namespace Core.Departments.AddDepartmentLocation
{
    public record AddDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand
    {
    }
}
