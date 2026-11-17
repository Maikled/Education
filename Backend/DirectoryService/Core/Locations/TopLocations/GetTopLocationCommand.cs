using Core.Interfaces;

namespace Core.Locations.TopLocations
{
    public record GetTopLocationCommand(int Limit) : ICommand;
}
