using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using Core.Positions.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Core.Positions.DeletePosition
{
    internal partial class DeletePositionHandler : ICommandHandler<DeletePositionCommand>
    {
        private readonly IPositionsRepository _positionsRepository;
        private readonly IDepartmentPositionsRepository _departmentPositionsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<DeletePositionHandler> _logger;

        public DeletePositionHandler(IPositionsRepository positionsRepository, IDepartmentPositionsRepository departmentPositionsRepository, ITransactionManager transactionManager, ILogger<DeletePositionHandler> logger)
        {
            _positionsRepository = positionsRepository;
            _departmentPositionsRepository = departmentPositionsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(DeletePositionCommand command, CancellationToken cancellationToken)
        {
            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var existPosition = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
            if (existPosition == null)
                return AppError.NotFound("position", "Position not found.");

            var isAttachedToDepartment = await _departmentPositionsRepository.ExistByPositionIdAsync(command.PositionId, cancellationToken);
            if (isAttachedToDepartment)
                return AppError.Conflict("position", "Position is attached to one or more departments.");

            await _positionsRepository.RemoveAsync(existPosition, cancellationToken);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogPositionDeleted(command.PositionId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} deleted")]
        private partial void LogPositionDeleted(Guid positionId);
    }
}
