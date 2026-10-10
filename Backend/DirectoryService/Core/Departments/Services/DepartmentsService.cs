using Contracts.DTOs;
using Core.Departments.Exceptions;
using Core.Departments.Interfaces;
using Core.Locations.Exceptions;
using Core.Locations.Interfaces;
using Domain.Entities;
using Domain.Entities.AssociativeEntities;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Departments.Services
{
    internal class DepartmentsService : IDepartmentService
    {
        private readonly IValidator<CreateDepartmentDto> _createDepartmentValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateDepartmentValidator;
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly ILocationsRepository _locationsRepository;
        private readonly IDepartmentLocationsRepository _departmentLocationsRepository;

        public DepartmentsService(IValidator<CreateDepartmentDto> createDepartmentValidator, IValidator<UpdateDepartmentDto> updateDepartmentValidator, IDepartmentsRepository departmentsRepository, ILocationsRepository locationsRepository, IDepartmentLocationsRepository departmentLocationsRepository)
        {
            _createDepartmentValidator = createDepartmentValidator;
            _updateDepartmentValidator = updateDepartmentValidator;
            _departmentsRepository = departmentsRepository;
            _locationsRepository = locationsRepository;
            _departmentLocationsRepository = departmentLocationsRepository;
        }

        public async Task<Guid> CreateDepartmentAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createDepartmentValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new DepartmentValidationException(validationResult.Errors.Select(p => p.ErrorMessage));

            Department department;
            if (dto.ParentId.HasValue)
            {
                var parentDepartment = await _departmentsRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
                if (parentDepartment == null)
                    throw new DepartmentNotFoundException(dto.ParentId.Value);

                department = Department.CreateChild(parentDepartment, Name.Create(dto.Name), Slug.Create(dto.Slug));
            }
            else
            {
                var departmentSlug = Slug.Create(dto.Slug);
                department = Department.Create(Name.Create(dto.Name), departmentSlug, DepartmentPath.Create(departmentSlug), null);
            }

            if (dto.locationsIds != null)
            {
                var locationsExists = await _locationsRepository.ExistAll(dto.locationsIds, cancellationToken);
                if (!locationsExists)
                    throw new LocationsNotExistsException(dto.locationsIds);

                await _departmentsRepository.AddWithLocationsAsync(department, dto.locationsIds.Select(p => DepartmentLocation.Create(department.Id, p, !dto.ParentId.HasValue)), cancellationToken);
            }
            else
            {
                await _departmentsRepository.AddAsync(department, cancellationToken);
            }

            return department.Id;
        }

        public async Task UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateDepartmentValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new DepartmentValidationException(validationResult.Errors.Select(p => p.ErrorMessage));

            var existDeparment = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
            if (existDeparment == null)
                throw new DepartmentNotFoundException(departmentId);

            var departmentSlug = Slug.Create(dto.Slug);

            DepartmentPath departmentPath;
            if (dto.ParentId.HasValue)
            {
                if (dto.ParentId.Value == departmentId)
                    throw new DepartmentParentConflictException(departmentId, dto.ParentId.Value);

                var parentDepartment = await _departmentsRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
                if (parentDepartment == null)
                    throw new DepartmentNotFoundException(dto.ParentId.Value);

                if (parentDepartment.Path.IsDescendantOf(existDeparment.Path))
                    throw new DepartmentsDescendantException(departmentId, dto.ParentId.Value);

                departmentPath = parentDepartment.Path.AppendPath(departmentSlug);
            }
            else
            {
                departmentPath = DepartmentPath.Create(departmentSlug);
            }
            
            existDeparment.Update(Name.Create(dto.Name), departmentSlug, departmentPath, dto.ParentId);

            await _departmentsRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task AddLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            var existDeparment = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
            if (existDeparment == null)
                throw new DepartmentNotFoundException(departmentId);

            var existLocation = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
            if (existLocation == null)
                throw new LocationNotFoundException(locationId);

            var departmentLocations = await _departmentLocationsRepository.GetAsync(departmentId, locationId, cancellationToken);
            if (departmentLocations != null)
                throw new DepartmentLocationExistsException(departmentId, locationId);

            var departmentLocation = DepartmentLocation.Create(departmentId, locationId, false);

            await _departmentLocationsRepository.AddAsync(departmentLocation, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            var departmentLocations = await _departmentLocationsRepository.GetAsync(departmentId, locationId, cancellationToken);
            if (departmentLocations == null)
                throw new DepartmentLocationNotExistException(departmentId, locationId);

            await _departmentLocationsRepository.RemoveAsync(departmentLocations, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
