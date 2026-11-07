using Contracts.DTOs;
using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Core.Departments.UpdateDepartment
{
    internal partial class UpdateDepartmentHandler : ICommandHandler<UpdateDepartmentCommand>
    {
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ILogger<UpdateDepartmentHandler> _logger;

        public UpdateDepartmentHandler(IDepartmentsRepository departmentsRepository, ITransactionManager transactionManager, ILogger<UpdateDepartmentHandler> logger)
        {
            _departmentsRepository = departmentsRepository;
            _transactionManager = transactionManager;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(UpdateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var existDepartment = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
            if (existDepartment == null)
                return AppError.NotFound("department", "Department not found.");

            var departmentSlugResult = Slug.Create(command.UpdateDepartmentDto.Slug);
            if (departmentSlugResult.IsFailure)
                return departmentSlugResult.Error;

            var departmentNameResult = Name.Create(command.UpdateDepartmentDto.Name);
            if (departmentNameResult.IsFailure)
                return departmentNameResult.Error;

            Result<DepartmentPath, AppError> departmentPathResult;
            if (command.UpdateDepartmentDto.ParentId.HasValue)
            {
                if (command.UpdateDepartmentDto.ParentId.Value == command.DepartmentId)
                    return AppError.Conflict("department", "Department cannot be its own parent.");

                var parentDepartment = await _departmentsRepository.GetByIdAsync(command.UpdateDepartmentDto.ParentId.Value, cancellationToken);
                if (parentDepartment == null)
                    return AppError.NotFound("department", "Parent department not found.");

                var isDescendantResult = parentDepartment.Path.IsDescendantOf(existDepartment.Path);
                if (isDescendantResult.IsFailure)
                    return isDescendantResult.Error;

                if (isDescendantResult.Value)
                    return AppError.Conflict("department", "Parent department cannot be a descendant of the current department.");

                departmentPathResult = parentDepartment.Path.AppendPath(departmentSlugResult.Value);
            }
            else
            {
                departmentPathResult = DepartmentPath.Create(departmentSlugResult.Value);
            }

            if (departmentPathResult.IsFailure)
                return departmentPathResult.Error;

            var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
            if (beginTransactionResult.IsFailure)
                return beginTransactionResult.Error;

            await using var transaction = beginTransactionResult.Value;

            existDepartment.Update(departmentNameResult.Value, departmentSlugResult.Value, departmentPathResult.Value, command.UpdateDepartmentDto.ParentId);

            var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            var commitResult = await transaction.CommitAsync(cancellationToken);
            if (commitResult.IsFailure)
                return commitResult.Error;

            LogDepartmentUpdated(command.DepartmentId, command.UpdateDepartmentDto.ParentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} updated with parent {ParentId}")]
        private partial void LogDepartmentUpdated(Guid departmentId, Guid? parentId);
    }
}
