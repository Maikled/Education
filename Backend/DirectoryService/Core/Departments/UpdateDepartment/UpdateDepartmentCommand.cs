using Contracts.DTOs;
using Core.Interfaces;

namespace Core.Departments.UpdateDepartment
{
    public record UpdateDepartmentCommand(UpdateDepartmentDto UpdateDepartmentDto, Guid DepartmentId) : ICommand
    {
    }
}
