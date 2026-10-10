using Contracts.DTOs;
using Core.Locations.Exceptions;
using Core.Locations.Interfaces;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Locations.Services
{
    internal sealed class LocationsService : ILocationsService
    {
        private readonly IValidator<CreateLocationDto> _createLocationValidator;
        private readonly IValidator<UpdateLocationDto> _updateLocationValidator;
        private readonly ILocationsRepository _locationsRepository;

        public LocationsService(IValidator<CreateLocationDto> createLocationValidator, IValidator<UpdateLocationDto> updateLocationValidator, ILocationsRepository locationsRepository)
        {
            _createLocationValidator = createLocationValidator;
            _updateLocationValidator = updateLocationValidator;
            _locationsRepository = locationsRepository;
        }

        public async Task<Guid> CreateLocationAsync(CreateLocationDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createLocationValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var locationName = Name.Create(dto.Name);
            var locationAddress = Address.Create(dto.Country, dto.State, dto.City, dto.Street, dto.BuildingNumber);

            var isExistLocationWithName = await _locationsRepository.ExistWithNameAsync(locationName, cancellationToken);
            if (isExistLocationWithName)
                throw new LocationNameException(locationName.Value);

            var location = Location.Create(locationName, locationAddress);

            await _locationsRepository.AddAsync(location, cancellationToken);

            return location.Id;
        }

        public async Task UpdateLocationAsync(Guid locationId, UpdateLocationDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateLocationValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existLocation = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
            if (existLocation == null)
                throw new LocationNotExistException(locationId);

            existLocation.Update(Name.Create(dto.Name), Address.Create(dto.Country, dto.State, dto.City, dto.Street, dto.BuildingNumber));

            await _locationsRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
