using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using Core.Positions.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities.AssociativeEntities;
using Microsoft.Extensions.Logging;

namespace Core.Departments.AddDepartmentPosition
{
    internal partial class AddDepartmentPositionHandler : ICommandHandler<AddDepartmentPositionCommand>
    {
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly IPositionsRepository _positionsRepository;
        private readonly IDepartmentPositionsRepository _departmentPositionsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<AddDepartmentPositionHandler> _logger;

        public AddDepartmentPositionHandler(IDepartmentsRepository departmentsRepository, IPositionsRepository positionsRepository, IDepartmentPositionsRepository departmentPositionsRepository, ITransactionManager transactionManager, ILogger<AddDepartmentPositionHandler> logger)
        {
            _departmentsRepository = departmentsRepository;
            _positionsRepository = positionsRepository;
            _departmentPositionsRepository = departmentPositionsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(AddDepartmentPositionCommand command, CancellationToken cancellationToken)
        {
            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var existDepartment = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
            if (existDepartment == null)
                return AppError.NotFound("department", "Department not found.");

            var existPosition = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
            if (existPosition == null)
                return AppError.NotFound("position", "Position not found.");

            var departmentPosition = await _departmentPositionsRepository.GetAsync(command.DepartmentId, command.PositionId, cancellationToken);
            if (departmentPosition != null)
                return AppError.Conflict("department_position", "Department-position relationship already exists.");

            var departmentPositionResult = DepartmentPosition.Create(command.DepartmentId, command.PositionId);
            if (departmentPositionResult.IsFailure)
                return departmentPositionResult.Error;

            await _departmentPositionsRepository.AddAsync(departmentPositionResult.Value, cancellationToken);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogPositionAttached(command.PositionId, command.DepartmentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} attached to department {DepartmentId}")]
        private partial void LogPositionAttached(Guid positionId, Guid departmentId);
    }
}
