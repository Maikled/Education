using Contracts.DTOs;
using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Core.Locations.Interfaces
{
    public interface ILocationsService
    {
        public Task<Result<Guid, AppError>> CreateLocationAsync(CreateLocationDto dto, CancellationToken cancellationToken);
        public Task<UnitResult<AppError>> UpdateLocationAsync(Guid locationId, UpdateLocationDto dto, CancellationToken cancellationToken);
    }
}
