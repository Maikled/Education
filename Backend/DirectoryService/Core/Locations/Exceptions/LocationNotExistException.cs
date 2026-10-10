using Contracts.Errors;

namespace Core.Locations.Exceptions
{
    public class LocationNotExistException : AppException
    {
        public LocationNotExistException(Guid locationId) : base(AppError.NotFound("location.notfound", $"Location with ID {locationId} does not exist."))
        {
            
        }
    }
}
