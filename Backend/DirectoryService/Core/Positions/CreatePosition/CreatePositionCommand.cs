using Contracts.DTOs;
using Core.Interfaces;

namespace Core.Positions.CreatePosition
{
    public record CreatePositionCommand(CreatePositionDto CreatePositionDto) : ICommand
    {
    }
}
