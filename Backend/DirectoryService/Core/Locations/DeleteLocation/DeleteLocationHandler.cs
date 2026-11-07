using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Core.Locations.DeleteLocation
{
    internal partial class DeleteLocationHandler : ICommandHandler<DeleteLocationCommand>
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly IDepartmentLocationsRepository _departmentLocationsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<DeleteLocationHandler> _logger;

        public DeleteLocationHandler(ILocationsRepository locationsRepository, IDepartmentLocationsRepository departmentLocationsRepository, ITransactionManager transactionManager, ILogger<DeleteLocationHandler> logger)
        {
            _locationsRepository = locationsRepository;
            _departmentLocationsRepository = departmentLocationsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(DeleteLocationCommand command, CancellationToken cancellationToken)
        {
            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var existLocation = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (existLocation == null)
                return AppError.NotFound("location", "Location not found.");

            var isAttachedToDepartment = await _departmentLocationsRepository.ExistByLocationIdAsync(command.LocationId, cancellationToken);
            if (isAttachedToDepartment)
                return AppError.Conflict("location", "Location is attached to one or more departments.");

            await _locationsRepository.RemoveAsync(existLocation, cancellationToken);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogLocationDeleted(command.LocationId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} deleted")]
        private partial void LogLocationDeleted(Guid locationId);
    }
}
