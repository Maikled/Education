using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Core.Departments.RemoveDepartmentPosition
{
    internal partial class RemoveDepartmentPositionHandler : ICommandHandler<RemoveDepartmentPositionCommand>
    {
        private readonly IDepartmentPositionsRepository _departmentPositionsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<RemoveDepartmentPositionHandler> _logger;

        public RemoveDepartmentPositionHandler(IDepartmentPositionsRepository departmentPositionsRepository, ITransactionManager transactionManager, ILogger<RemoveDepartmentPositionHandler> logger)
        {
            _departmentPositionsRepository = departmentPositionsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(RemoveDepartmentPositionCommand command, CancellationToken cancellationToken)
        {
            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var departmentPosition = await _departmentPositionsRepository.GetAsync(command.DepartmentId, command.PositionId, cancellationToken);
            if (departmentPosition == null)
                return AppError.NotFound("department_position", "Department-position relationship not found.");

            await _departmentPositionsRepository.RemoveAsync(departmentPosition, cancellationToken);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogPositionDetached(command.PositionId, command.DepartmentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} detached from department {DepartmentId}")]
        private partial void LogPositionDetached(Guid positionId, Guid departmentId);
    }
}
