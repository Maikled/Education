using Core.Interfaces;

namespace Core.Departments.RemoveDepartmentLocation
{
    public record RemoveDepartmentLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand
    {
    }
}
