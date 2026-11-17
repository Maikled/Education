using Core.Interfaces;

namespace Core.Departments.DeleteDepartment
{
    public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand
    {
    }
}
