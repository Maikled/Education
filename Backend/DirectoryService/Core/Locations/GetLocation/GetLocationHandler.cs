using Contracts.DTOs;
using Contracts.Errors;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;

namespace Core.Locations.GetLocation
{
    internal class GetLocationHandler : ICommandHandler<LocationDto, GetLocationCommand>
    {
        private readonly ILocationReadRepository _locationReadRepository;

        public GetLocationHandler(ILocationReadRepository locationReadRepository)
        {
            _locationReadRepository = locationReadRepository;
        }

        public async Task<Result<LocationDto, AppError>> HandlerAsync(GetLocationCommand command, CancellationToken cancellationToken)
        {
            var location = await _locationReadRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (location == null)
            {
                return AppError.NotFound("location", "Location not found");
            }

            var address = new AddressDto(location.Address.Country, location.Address.State, location.Address.City, location.Address.Street, location.Address.BuildingNumber);

            return Result.Success<LocationDto, AppError>(new LocationDto(location.Id, location.Name.Value, address, location.CreatedAt, location.UpdatedAt));
        }
    }
}
