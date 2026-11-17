using Core.Interfaces;

namespace Core.Departments.RemoveDepartmentPosition
{
    public record RemoveDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand
    {
    }
}
