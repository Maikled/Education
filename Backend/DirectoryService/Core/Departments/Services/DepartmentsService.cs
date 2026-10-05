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
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly ILocationsRepository _locationsRepository;

        public DepartmentsService(IValidator<CreateDepartmentDto> createDepartmentValidator, IDepartmentsRepository departmentsRepository, ILocationsRepository locationsRepository)
        {
            _createDepartmentValidator = createDepartmentValidator;
            _departmentsRepository = departmentsRepository;
            _locationsRepository = locationsRepository;
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
                List<DepartmentLocation> departmentLocations = new List<DepartmentLocation>();
                foreach (var locationId in dto.locationsIds)
                {
                    var locationExists = await _locationsRepository.ExistById(locationId, cancellationToken);
                    if (!locationExists)
                        throw new InvalidOperationException($"Location with ID {locationId} does not exist.");

                    departmentLocations.Add(DepartmentLocation.Create(department.Id, locationId, !dto.ParentId.HasValue));
                }

                await _departmentsRepository.AddWithLocationsAsync(department, departmentLocations, cancellationToken);
            }
            else
            {
                await _departmentsRepository.AddAsync(department, cancellationToken);
            }

            return department.Id;
        }
    }
}
