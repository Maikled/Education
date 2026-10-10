using Contracts.Errors;

namespace Core.Locations.Exceptions
{
    public class LocationNameException : AppException
    {
        public LocationNameException(string name) : base(AppError.Conflict("location.name", $"Location with name '{name}' already exists."))
        {
            
        }
    }
}
