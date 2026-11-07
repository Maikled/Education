using Core.Interfaces;

namespace Core.Locations.DeleteLocation
{
    public record DeleteLocationCommand(Guid LocationId) : ICommand
    {
    }
}
