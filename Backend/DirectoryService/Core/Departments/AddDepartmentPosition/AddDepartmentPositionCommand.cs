using Core.Interfaces;

namespace Core.Departments.AddDepartmentPosition
{
    public record AddDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand
    {
    }
}
