using Contracts.DTOs;
using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Domain.Entities.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Departments.Services
{
    internal partial class DepartmentsService : IDepartmentService
    {
        private readonly IValidator<CreateDepartmentDto> _createDepartmentValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateDepartmentValidator;
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly ILocationsRepository _locationsRepository;
        private readonly IDepartmentLocationsRepository _departmentLocationsRepository;
        private readonly ILogger<DepartmentsService> _logger;

        public DepartmentsService(IValidator<CreateDepartmentDto> createDepartmentValidator, IValidator<UpdateDepartmentDto> updateDepartmentValidator, IDepartmentsRepository departmentsRepository, ILocationsRepository locationsRepository, IDepartmentLocationsRepository departmentLocationsRepository, ILogger<DepartmentsService> logger)
        {
            _createDepartmentValidator = createDepartmentValidator;
            _updateDepartmentValidator = updateDepartmentValidator;
            _departmentsRepository = departmentsRepository;
            _locationsRepository = locationsRepository;
            _departmentLocationsRepository = departmentLocationsRepository;
            _logger = logger;
        }

        public async Task<Result<Guid, AppError>> CreateDepartmentAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createDepartmentValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var departmentNameResult = Name.Create(dto.Name);
            if (departmentNameResult.IsFailure)
                return departmentNameResult.Error;

            var departmentsSlugResult = Slug.Create(dto.Slug);
            if (departmentsSlugResult.IsFailure)
                return departmentsSlugResult.Error;

            Result<Department, AppError> departmentResult;
            if (dto.ParentId.HasValue)
            {
                var parentDepartment = await _departmentsRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
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

            if (dto.locationsIds != null)
            {
                var locationsExists = await _locationsRepository.ExistAll(dto.locationsIds, cancellationToken);
                if (!locationsExists)
                    return AppError.NotFound("locations", "One or more locations not found.");

                var departmentLocationResults = dto.locationsIds.Select(p => DepartmentLocation.Create(departmentResult.Value.Id, p, !dto.ParentId.HasValue)).ToList();
                if (departmentLocationResults.Any(p => p.IsFailure))
                    return AppError.FromErrors(departmentLocationResults.Where(p => p.IsFailure).Select(p => p.Error).ToArray());


                await _departmentsRepository.AddWithLocationsAsync(departmentResult.Value, departmentLocationResults.Select(p => p.Value), cancellationToken);
            }
            else
            {
                await _departmentsRepository.AddAsync(departmentResult.Value, cancellationToken);
            }

            LogDepartmentCreated(departmentResult.Value.Id, dto.ParentId);

            return departmentResult.Value.Id;
        }

        public async Task<UnitResult<AppError>> UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateDepartmentValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                return AppError.FromErrors([.. validationResult.Errors.Select(p => AppError.Validation(p.ErrorCode, p.ErrorMessage))]);

            var existDepartment = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
            if (existDepartment == null)
                return AppError.NotFound("department", "Department not found.");

            var departmentSlugResult = Slug.Create(dto.Slug);
            if (departmentSlugResult.IsFailure)
                return departmentSlugResult.Error;

            var departmentNameResult = Name.Create(dto.Name);
            if (departmentNameResult.IsFailure)
                return departmentNameResult.Error;

            Result<DepartmentPath, AppError> departmentPathResult;
            if (dto.ParentId.HasValue)
            {
                if (dto.ParentId.Value == departmentId)
                    return AppError.Conflict("department", "Department cannot be its own parent.");

                var parentDepartment = await _departmentsRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
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
            
            existDepartment.Update(departmentNameResult.Value, departmentSlugResult.Value, departmentPathResult.Value, dto.ParentId);

            await _departmentsRepository.SaveChangesAsync(cancellationToken);

            LogDepartmentUpdated(departmentId, dto.ParentId);

            return UnitResult.Success<AppError>();
        }

        public async Task<UnitResult<AppError>> AddLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            var existDeparment = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
            if (existDeparment == null)
                return AppError.NotFound("department", "Department not found.");

            var existLocation = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
            if (existLocation == null)
                return AppError.NotFound("location", "Location not found.");

            var departmentLocations = await _departmentLocationsRepository.GetAsync(departmentId, locationId, cancellationToken);
            if (departmentLocations != null)
                return AppError.Conflict("department_location", "Department-location relationship already exists.");

            var departmentLocationResult = DepartmentLocation.Create(departmentId, locationId, false);
            if (departmentLocationResult.IsFailure)
                return departmentLocationResult.Error;

            await _departmentLocationsRepository.AddAsync(departmentLocationResult.Value, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);

            LogLocationAttached(locationId, departmentId);

            return UnitResult.Success<AppError>();
        }

        public async Task<UnitResult<AppError>> RemoveLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            var departmentLocations = await _departmentLocationsRepository.GetAsync(departmentId, locationId, cancellationToken);
            if (departmentLocations == null)
                return AppError.NotFound("department_location", "Department-location relationship not found.");

            await _departmentLocationsRepository.RemoveAsync(departmentLocations, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);

            LogLocationDetached(locationId, departmentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} created with parent {ParentId}")]
        private partial void LogDepartmentCreated(Guid departmentId, Guid? parentId);

        [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} updated with parent {ParentId}")]
        private partial void LogDepartmentUpdated(Guid departmentId, Guid? parentId);

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} attached to department {DepartmentId}")]
        private partial void LogLocationAttached(Guid locationId, Guid departmentId);

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} detached from department {DepartmentId}")]
        private partial void LogLocationDetached(Guid locationId, Guid departmentId);
    }
}
