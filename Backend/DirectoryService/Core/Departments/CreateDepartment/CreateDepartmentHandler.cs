using Contracts.DTOs;
using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Domain.Entities.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Departments.CreateDepartment
{
    internal partial class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
    {
        private readonly IValidator<CreateDepartmentDto> _createDepartmentValidator;
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly ILocationsRepository _locationsRepository;
        private readonly ILogger<CreateDepartmentHandler> _logger;

        public CreateDepartmentHandler(IValidator<CreateDepartmentDto> createDepartmentValidator, IDepartmentsRepository departmentsRepository, ILocationsRepository locationsRepository, ILogger<CreateDepartmentHandler> logger)
        {
            _createDepartmentValidator = createDepartmentValidator;
            _departmentsRepository = departmentsRepository;
            _locationsRepository = locationsRepository;
            _logger = logger;
        }

        public async Task<Result<Guid, AppError>> HandlerAsync(CreateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _createDepartmentValidator.ValidateAsync(command.CreateDepartmentDto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var departmentNameResult = Name.Create(command.CreateDepartmentDto.Name);
            if (departmentNameResult.IsFailure)
                return departmentNameResult.Error;

            var departmentsSlugResult = Slug.Create(command.CreateDepartmentDto.Slug);
            if (departmentsSlugResult.IsFailure)
                return departmentsSlugResult.Error;

            Result<Department, AppError> departmentResult;
            if (command.CreateDepartmentDto.ParentId.HasValue)
            {
                var parentDepartment = await _departmentsRepository.GetByIdAsync(command.CreateDepartmentDto.ParentId.Value, cancellationToken);
                if (parentDepartment == null)
                    return AppError.NotFound("department", "Parent department not found.");

                departmentResult = Department.CreateChild(parentDepartment, departmentNameResult.Value, departmentsSlugResult.Value);
            }
            else
            {
                var departmentsPathResult = DepartmentPath.Create(departmentsSlugResult.Value);
                if (departmentsPathResult.IsFailure)
                    return departmentsPathResult.Error;

                departmentResult = Department.Create(departmentNameResult.Value, departmentsSlugResult.Value, departmentsPathResult.Value, null);
            }

            if (departmentResult.IsFailure)
                return departmentResult.Error;

            if (command.CreateDepartmentDto.locationsIds != null)
            {
                var locationsExists = await _locationsRepository.ExistAll(command.CreateDepartmentDto.locationsIds, cancellationToken);
                if (!locationsExists)
                    return AppError.NotFound("locations", "One or more locations not found.");

                var departmentLocationResults = command.CreateDepartmentDto.locationsIds.Select(p => DepartmentLocation.Create(departmentResult.Value.Id, p, !command.CreateDepartmentDto.ParentId.HasValue)).ToList();
                if (departmentLocationResults.Any(p => p.IsFailure))
                    return AppError.FromErrors(departmentLocationResults.Where(p => p.IsFailure).Select(p => p.Error).ToArray());


                await _departmentsRepository.AddWithLocationsAsync(departmentResult.Value, departmentLocationResults.Select(p => p.Value), cancellationToken);
            }
            else
            {
                await _departmentsRepository.AddAsync(departmentResult.Value, cancellationToken);
            }

            LogDepartmentCreated(departmentResult.Value.Id, command.CreateDepartmentDto.ParentId);

            return departmentResult.Value.Id;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} created with parent {ParentId}")]
        private partial void LogDepartmentCreated(Guid departmentId, Guid? parentId);
    }
}
