using Contracts;
using Contracts.DTOs;
using Contracts.Errors;
using Core.Enums;
using Core.Interfaces;
using Core.Locations.GetLocations.Enums;
using Core.Locations.Interfaces;
using CSharpFunctionalExtensions;

namespace Core.Locations.GetLocations
{
    internal class GetLocationsHandler : ICommandHandler<PagedResult<LocationListItemDto>, GetLocationsCommand>
    {
        private readonly ILocationReadRepository _locationReadRepository;

        public GetLocationsHandler(ILocationReadRepository locationReadRepository)
        {
            _locationReadRepository = locationReadRepository;
        }

        public async Task<Result<PagedResult<LocationListItemDto>, AppError>> HandlerAsync(GetLocationsCommand command, CancellationToken cancellationToken)
        {
            LocationSortFields? sortField = null;
            if (!string.IsNullOrWhiteSpace(command.SortField))
            {
                if (!Enum.TryParse<LocationSortFields>(command.SortField, true, out var parsedSortField))
                    return Result.Failure<PagedResult<LocationListItemDto>, AppError>(AppError.Validation("locations.sortField", "Invalid sort field"));

                sortField = parsedSortField;
            }

            if (!Enum.TryParse<SortDirection>(command.SortDirection, true, out var parsedSortDirection))
                return Result.Failure<PagedResult<LocationListItemDto>, AppError>(AppError.Validation("locations.sortDirection", "Invalid sort direction"));

            if (command.MinDepartments < 0)
                return Result.Failure<PagedResult<LocationListItemDto>, AppError>(AppError.Validation("locations.minDepartments", "Min departments must be greater than or equal to 0"));

            if (command.Page < 1)
                return Result.Failure<PagedResult<LocationListItemDto>, AppError>(AppError.Validation("locations.page", "Page must be greater than 0"));

            if (command.PageSize < 1)
                return Result.Failure<PagedResult<LocationListItemDto>, AppError>(AppError.Validation("locations.pageSize", "Page size must be greater than 0"));

            if (command.PageSize > 100)
                return Result.Failure<PagedResult<LocationListItemDto>, AppError>(AppError.Validation("locations.pageSize", "Page size must be less than or equal to 100"));

            return await _locationReadRepository.GetLocationsAsync(command.Search, command.MinDepartments, sortField, parsedSortDirection, command.Page, command.PageSize, cancellationToken);
        }
    }
}
