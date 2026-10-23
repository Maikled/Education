using Contracts.Errors;
using Core.Departments.AddDepartmentLocation;
using Core.Departments.Interfaces;
using Core.Interfaces;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;
using Domain.Entities.AssociativeEntities;
using Microsoft.Extensions.Logging;

namespace Core.Departments.AddDepartmentLocations
{
    internal partial class AddDepartmentLocationHandler : ICommandHandler<AddDepartmentLocationCommand>
    {
        private readonly IDepartmentsRepository _departmentsRepository;
        private readonly ILocationsRepository _locationsRepository;
        private readonly IDepartmentLocationsRepository _departmentLocationsRepository;
        private readonly ILogger<AddDepartmentLocationHandler> _logger;

        public AddDepartmentLocationHandler(IDepartmentsRepository departmentsRepository, ILocationsRepository locationsRepository, IDepartmentLocationsRepository departmentLocationsRepository, ILogger<AddDepartmentLocationHandler> logger)
        {
            _departmentsRepository = departmentsRepository;
            _locationsRepository = locationsRepository;
            _departmentLocationsRepository = departmentLocationsRepository;
            _logger = logger;
        }

        public async Task<UnitResult<AppError>> HandlerAsync(AddDepartmentLocationCommand command, CancellationToken cancellationToken)
        {
            var existDeparment = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
            if (existDeparment == null)
                return AppError.NotFound("department", "Department not found.");

            var existLocation = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
            if (existLocation == null)
                return AppError.NotFound("location", "Location not found.");

            var departmentLocations = await _departmentLocationsRepository.GetAsync(command.DepartmentId, command.LocationId, cancellationToken);
            if (departmentLocations != null)
                return AppError.Conflict("department_location", "Department-location relationship already exists.");

            var departmentLocationResult = DepartmentLocation.Create(command.DepartmentId, command.LocationId, false);
            if (departmentLocationResult.IsFailure)
                return departmentLocationResult.Error;

            await _departmentLocationsRepository.AddAsync(departmentLocationResult.Value, cancellationToken);
            await _departmentLocationsRepository.SaveChangesAsync(cancellationToken);

            LogLocationAttached(command.LocationId, command.DepartmentId);

            return UnitResult.Success<AppError>();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} attached to department {DepartmentId}")]
        private partial void LogLocationAttached(Guid locationId, Guid departmentId);
    }
}
