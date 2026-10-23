using Contracts.DTOs;
using Core.Interfaces;

namespace Core.Locations.CreateLocation
{
    public record CreateLocationCommand(CreateLocationDto CreateLocationDto) : ICommand
    {
    }
}
