using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Core.Departments.DeleteDepartment
{
    internal partial class DeleteDepartmentHandler : ICommandHandler<DeleteDepartmentCommand>
    {
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly IDepartmentLocationsRepository _departmentLocationsRepository;
        private readonly IDepartmentPositionsRepository _departmentPositionsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<DeleteDepartmentHandler> _logger;

        public DeleteDepartmentHandler(IDepartmentsRepository departmentsRepository, IDepartmentLocationsRepository departmentLocationsRepository, IDepartmentPositionsRepository departmentPositionsRepository, ITransactionManager transactionManager, ILogger<DeleteDepartmentHandler> logger)
        {
            _departmentsRepository = departmentsRepository;
            _departmentLocationsRepository = departmentLocationsRepository;
            _departmentPositionsRepository = departmentPositionsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(DeleteDepartmentCommand command, CancellationToken cancellationToken)
        {
            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            var existDepartment = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
            if (existDepartment == null)
                return AppError.NotFound("department", "Department not found.");

            var hasChildren = await _departmentsRepository.ExistChildrenAsync(command.DepartmentId, cancellationToken);
            if (hasChildren)
                return AppError.Conflict("department", "Department has child departments.");

            await _departmentLocationsRepository.RemoveByDepartmentIdAsync(command.DepartmentId, cancellationToken);
            await _departmentPositionsRepository.RemoveByDepartmentIdAsync(command.DepartmentId, cancellationToken);
            await _departmentsRepository.RemoveAsync(existDepartment, cancellationToken);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogDepartmentDeleted(command.DepartmentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} deleted")]
        private partial void LogDepartmentDeleted(Guid departmentId);
    }
}
