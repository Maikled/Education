using Core.Interfaces;

namespace Core.Locations.GetLocation
{
    public record GetLocationCommand(Guid LocationId) : ICommand;
}
