using Core.Interfaces;

namespace Core.Departments.GetDepartment
{
    public record GetDepartmentCommand(Guid DepartmentId) : ICommand;
}
