using Contracts.Errors;

namespace Core.Locations.Exceptions
{
    public class LocationsNotExistsException : AppException
    {
        public LocationsNotExistsException(IEnumerable<Guid> locationIds) : base(AppError.NotFound("locations.notfound", $"One or more locations with IDs {string.Join(", ", locationIds)} do not exist."))
        {
            
        }
    }
}
