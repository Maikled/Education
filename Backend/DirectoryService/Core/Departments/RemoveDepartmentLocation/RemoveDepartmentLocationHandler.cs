using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Core.Departments.RemoveDepartmentLocation
{
    internal partial class RemoveDepartmentLocationHandler : ICommandHandler<RemoveDepartmentLocationCommand>
    {
        private readonly IDepartmentLocationsRepository _departmentLocationsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<RemoveDepartmentLocationHandler> _logger;

        public RemoveDepartmentLocationHandler(IDepartmentLocationsRepository departmentLocationsRepository, ITransactionManager transactionManager, ILogger<RemoveDepartmentLocationHandler> logger)
        {
            _departmentLocationsRepository = departmentLocationsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(RemoveDepartmentLocationCommand command, CancellationToken cancellationToken)
        {
            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            using var transaction = beginTransactionResult.Value;

            var departmentLocations = await _departmentLocationsRepository.GetAsync(command.DepartmentId, command.LocationId, cancellationToken);
            if (departmentLocations == null)
                return AppError.NotFound("department_location", "Department-location relationship not found.");

            await _departmentLocationsRepository.RemoveAsync(departmentLocations, cancellationToken);

            var saveTransactionResult = await _transactionManager.SaveTransactionAsync(cancellationToken);
            if (saveTransactionResult.IsFailure)
            {
                transaction.Rollback();
                return saveTransactionResult.Error;
            }

            LogLocationDetached(command.LocationId, command.DepartmentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} detached from department {DepartmentId}")]
        private partial void LogLocationDetached(Guid locationId, Guid departmentId);
    }
}
