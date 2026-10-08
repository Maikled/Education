using Contracts.DTOs;
using Core.Departments.Interfaces;
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
                throw new ValidationException(validationResult.Errors);

            Department department;
            if (dto.ParentId.HasValue)
            {
                var parentDepartment = await _departmentsRepository.GetByIdAsync(dto.ParentId.Value, cancellationToken);
                if (parentDepartment == null)
                    throw new InvalidOperationException($"Parent department with ID {dto.ParentId} does not exist.");

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
                    throw new InvalidOperationException($"One or more locations do not exist.");

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
                throw new ValidationException(validationResult.Errors);

            var existDeparment = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
            if (existDeparment == null)
                throw new InvalidOperationException($"Department with ID {departmentId} does not exist.");

            var departmentSlug = Slug.Create(dto.Slug);

            DepartmentPath departmentPath;
            if (existDeparment.ParentId != null)
            {
                var parentDepartment = await _departmentsRepository.GetByIdAsync(existDeparment.ParentId.Value, cancellationToken);
                if (parentDepartment == null)
                    throw new InvalidOperationException($"Parent department with ID {existDeparment.ParentId} does not exist.");

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
            {
                throw new InvalidOperationException($"Department with ID {departmentId} does not exist.");
            }

            var existLocation = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
            if (existLocation == null)
            {
                throw new InvalidOperationException($"Location with ID {locationId} does not exist.");
            }

            var departmentLocations = await _departmentLocationsRepository.GetAsync(departmentId, locationId, cancellationToken);
            if (departmentLocations != null)
            {
                throw new InvalidOperationException($"Department location with IDs {departmentId} and {locationId} already exists.");
            }

            var departmentLocation = DepartmentLocation.Create(departmentId, locationId, false);

            await _departmentLocationsRepository.AddAsync(departmentLocation, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveLocationsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            var departmentLocations = await _departmentLocationsRepository.GetAsync(departmentId, locationId, cancellationToken);
            if (departmentLocations == null)
            {
                throw new InvalidOperationException($"Department location with IDs {departmentId} and {locationId} does not exist.");
            }

            await _departmentLocationsRepository.RemoveAsync(departmentLocations, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
