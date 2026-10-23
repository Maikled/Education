using Contracts.DTOs;
using Contracts.Errors;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Locations.CreateLocation
{
    internal partial class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
    {
        private readonly IValidator<CreateLocationDto> _createLocationValidator;
        private readonly ILocationsRepository _locationsRepository;
        private readonly ILogger<CreateLocationHandler> _logger;

        public CreateLocationHandler(IValidator<CreateLocationDto> createLocationValidator, ILocationsRepository locationsRepository, ILogger<CreateLocationHandler> logger)
        {
            _createLocationValidator = createLocationValidator;
            _locationsRepository = locationsRepository;
            _logger = logger;
        }

        public async Task<Result<Guid, AppError>> HandlerAsync(CreateLocationCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _createLocationValidator.ValidateAsync(command.CreateLocationDto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var locationNameResult = Name.Create(command.CreateLocationDto.Name);
            if (locationNameResult.IsFailure)
                return locationNameResult.Error;

            var locationAddressResult = Address.Create(command.CreateLocationDto.Address.Country, command.CreateLocationDto.Address.State, command.CreateLocationDto.Address.City, command.CreateLocationDto.Address.Street, command.CreateLocationDto.Address.BuildingNumber);
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

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} created")]
        private partial void LogLocationCreated(Guid locationId);
    }
}
