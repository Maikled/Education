using Contracts.DTOs;
using Core.Interfaces;

namespace Core.Positions.UpdatePosition
{
    public record UpdatePositionCommand(Guid PositionId, UpdatePositionDto UpdatePositionDto) : ICommand
    {
    }
}
