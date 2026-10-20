using Contracts.DTOs;
using Contracts.Errors;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Locations.Services
{
    internal sealed partial class LocationsService : ILocationsService
    {
        private readonly IValidator<CreateLocationDto> _createLocationValidator;
        private readonly IValidator<UpdateLocationDto> _updateLocationValidator;
        private readonly ILocationsRepository _locationsRepository;
        private readonly ILogger<LocationsService> _logger;

        public LocationsService(IValidator<CreateLocationDto> createLocationValidator, IValidator<UpdateLocationDto> updateLocationValidator, ILocationsRepository locationsRepository, ILogger<LocationsService> logger)
        {
            _createLocationValidator = createLocationValidator;
            _updateLocationValidator = updateLocationValidator;
            _locationsRepository = locationsRepository;
            _logger = logger;
        }

        public async Task<Result<Guid, AppError>> CreateLocationAsync(CreateLocationDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createLocationValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var locationNameResult = Name.Create(dto.Name);
            if (locationNameResult.IsFailure)
                return locationNameResult.Error;

            var locationAddressResult = Address.Create(dto.Address.Country, dto.Address.State, dto.Address.City, dto.Address.Street, dto.Address.BuildingNumber);
            if (locationAddressResult.IsFailure)
                return locationAddressResult.Error;

            var isExistLocationWithName = await _locationsRepository.ExistWithNameAsync(locationNameResult.Value, cancellationToken);
            if (isExistLocationWithName)
                return AppError.Conflict("location.name", "A location with the same name already exists.");

            var locationResult = Location.Create(locationNameResult.Value, locationAddressResult.Value);
            if (locationResult.IsFailure)
                return locationResult.Error;

            await _locationsRepository.AddAsync(locationResult.Value, cancellationToken);

            LogLocationCreated(locationResult.Value.Id);

            return locationResult.Value.Id;
        }

        public async Task<UnitResult<AppError>> UpdateLocationAsync(Guid locationId, UpdateLocationDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateLocationValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var locationNameResult = Name.Create(dto.Name);
            if (locationNameResult.IsFailure)
                return locationNameResult.Error;

            var locationAddressResult = Address.Create(dto.Address.Country, dto.Address.State, dto.Address.City, dto.Address.Street, dto.Address.BuildingNumber);
            if (locationAddressResult.IsFailure)
                return locationAddressResult.Error;

            var existLocation = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
            if (existLocation == null)
                return AppError.NotFound("location.not_found", $"Location with ID '{locationId}' does not exist.");

            existLocation.Update(locationNameResult.Value, locationAddressResult.Value);

            await _locationsRepository.SaveChangesAsync(cancellationToken);

            LogLocationUpdated(locationId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} created")]
        private partial void LogLocationCreated(Guid locationId);

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} updated")]
        private partial void LogLocationUpdated(Guid locationId);
    }
}
