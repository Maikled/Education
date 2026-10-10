using Contracts.Errors;

namespace Core.Locations.Exceptions
{
    public class LocationNotFoundException : AppException
    {
        public LocationNotFoundException(Guid locationId) : base(AppError.NotFound("location.notfound", $"Location with ID {locationId} does not exist."))
        {
            
        }
    }
}
