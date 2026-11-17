using Contracts.Errors;
using Core.Interfaces;
using Core.Positions.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Core.Positions.CreatePosition
{
    internal partial class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
    {
        private readonly IPositionsRepository _positionsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<CreatePositionHandler> _logger;

        public CreatePositionHandler(IPositionsRepository positionsRepository, ITransactionManager transactionManager, ILogger<CreatePositionHandler> logger)
        {
            _positionsRepository = positionsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<Result<Guid, AppError>> HandlerAsync(CreatePositionCommand command, CancellationToken cancellationToken)
        {
            var positionNameResult = Name.Create(command.CreatePositionDto.Name);
            if (positionNameResult.IsFailure)
                return positionNameResult.Error;

            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var isExistPositionWithName = await _positionsRepository.ExistWithNameAsync(positionNameResult.Value, cancellationToken);
            if (isExistPositionWithName)
                return AppError.Conflict("position.name", "A position with the same name already exists.");

            var positionResult = Position.Create(positionNameResult.Value);
            if (positionResult.IsFailure)
                return positionResult.Error;

            await _positionsRepository.AddAsync(positionResult.Value, cancellationToken);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogPositionCreated(positionResult.Value.Id);

            return positionResult.Value.Id;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} created")]
        private partial void LogPositionCreated(Guid positionId);
    }
}
