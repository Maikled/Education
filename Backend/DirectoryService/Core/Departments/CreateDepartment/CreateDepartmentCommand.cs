using Contracts.DTOs;
using Core.Interfaces;

namespace Core.Departments.CreateDepartment
{
    public record CreateDepartmentCommand(CreateDepartmentDto CreateDepartmentDto) : ICommand
    {

    }
}
