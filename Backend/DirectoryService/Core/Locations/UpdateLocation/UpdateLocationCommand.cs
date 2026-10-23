using Contracts.DTOs;
using Core.Interfaces;

namespace Core.Locations.UpdateLocation
{
    public record UpdateLocationCommand(Guid LocationId, UpdateLocationDto UpdateLocationDto) : ICommand
    {
    }
}
