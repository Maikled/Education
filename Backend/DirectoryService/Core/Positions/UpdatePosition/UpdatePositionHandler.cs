using Contracts.Errors;
using Core.Interfaces;
using Core.Positions.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Core.Positions.UpdatePosition
{
    internal partial class UpdatePositionHandler : ICommandHandler<UpdatePositionCommand>
    {
        private readonly IPositionsRepository _positionsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<UpdatePositionHandler> _logger;

        public UpdatePositionHandler(IPositionsRepository positionsRepository, ITransactionManager transactionManager, ILogger<UpdatePositionHandler> logger)
        {
            _positionsRepository = positionsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(UpdatePositionCommand command, CancellationToken cancellationToken)
        {
            var positionNameResult = Name.Create(command.UpdatePositionDto.Name);
            if (positionNameResult.IsFailure)
                return positionNameResult.Error;

            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var existPosition = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
            if (existPosition == null)
                return AppError.NotFound("position", "Position not found.");

            if (existPosition.Name != positionNameResult.Value)
            {
                var isExistPositionWithName = await _positionsRepository.ExistWithNameAsync(positionNameResult.Value, cancellationToken);
                if (isExistPositionWithName)
                    return AppError.Conflict("position.name", "A position with the same name already exists.");
            }

            var updateResult = existPosition.Update(positionNameResult.Value);
            if (updateResult.IsFailure)
                return updateResult.Error;

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogPositionUpdated(command.PositionId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} updated")]
        private partial void LogPositionUpdated(Guid positionId);
    }
}
