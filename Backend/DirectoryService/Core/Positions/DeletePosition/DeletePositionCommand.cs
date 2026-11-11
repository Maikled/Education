using Core.Interfaces;

namespace Core.Positions.DeletePosition
{
    public record DeletePositionCommand(Guid PositionId) : ICommand
    {
    }
}
