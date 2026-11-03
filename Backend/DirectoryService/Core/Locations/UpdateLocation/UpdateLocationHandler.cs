using Contracts.DTOs;
using Contracts.Errors;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Locations.UpdateLocation
{
    internal partial class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand>
    {
        private readonly IValidator<UpdateLocationDto> _updateLocationValidator;
        private readonly ILocationsRepository _locationsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<UpdateLocationHandler> _logger;

        public UpdateLocationHandler(IValidator<UpdateLocationDto> updateLocationValidator, ILocationsRepository locationsRepository,  ILogger<UpdateLocationHandler> logger, ITransactionManager transactionManager)
        {
            _updateLocationValidator = updateLocationValidator;
            _locationsRepository = locationsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(UpdateLocationCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _updateLocationValidator.ValidateAsync(command.UpdateLocationDto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var locationNameResult = Name.Create(command.UpdateLocationDto.Name);
            if (locationNameResult.IsFailure)
                return locationNameResult.Error;

            var locationAddressResult = Address.Create(command.UpdateLocationDto.Address.Country, command.UpdateLocationDto.Address.State, command.UpdateLocationDto.Address.City, command.UpdateLocationDto.Address.Street, command.UpdateLocationDto.Address.BuildingNumber);
            if (locationAddressResult.IsFailure)
                return locationAddressResult.Error;

            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var existLocation = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (existLocation == null)
                return AppError.NotFound("location.not_found", $"Location with ID '{command.LocationId}' does not exist.");

            existLocation.Update(locationNameResult.Value, locationAddressResult.Value);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogLocationUpdated(command.LocationId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} updated")]
        private partial void LogLocationUpdated(Guid locationId);
    }
}
