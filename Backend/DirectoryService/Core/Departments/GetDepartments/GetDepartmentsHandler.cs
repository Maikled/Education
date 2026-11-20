using Contracts;
using Contracts.DTOs;
using Contracts.Errors;
using Core.Departments.GetDepartments.Enums;
using Core.Departments.Interfaces;
using Core.Enums;
using Core.Interfaces;
using CSharpFunctionalExtensions;

namespace Core.Departments.GetDepartments
{
    internal class GetDepartmentsHandler : ICommandHandler<PagedResult<DepartmentListItemDto>, GetDepartmentsCommand>
    {
        private readonly IDepartmentReadRepository _departmentReadRepository;

        public GetDepartmentsHandler(IDepartmentReadRepository departmentReadRepository)
        {
            _departmentReadRepository = departmentReadRepository;
        }

        public async Task<Result<PagedResult<DepartmentListItemDto>, AppError>> HandlerAsync(GetDepartmentsCommand command, CancellationToken cancellationToken)
        {
            DepartmentSortFields? sortField = null;
            if (!string.IsNullOrWhiteSpace(command.SortField))
            {
                if (!Enum.TryParse<DepartmentSortFields>(command.SortField, out var parsedSortField))
                    return Result.Failure<PagedResult<DepartmentListItemDto>, AppError>(AppError.Validation("departments.sortField", "Invalid sort field"));
                
                sortField = parsedSortField;
            }

            if (!Enum.TryParse<SortDirection>(command.SortDirection, out var parsedSortDirection))
                return Result.Failure<PagedResult<DepartmentListItemDto>, AppError>(AppError.Validation("departments.sortDirection", "Invalid sort direction"));

            if (command.Page < 1)
                return Result.Failure<PagedResult<DepartmentListItemDto>, AppError>(AppError.Validation("departments.page", "Page must be greater than 0"));

            if (command.PageSize < 1)
                return Result.Failure<PagedResult<DepartmentListItemDto>, AppError>(AppError.Validation("departments.pageSize", "Page size must be greater than 0"));

            if (command.PageSize > 100)
                return Result.Failure<PagedResult<DepartmentListItemDto>, AppError>(AppError.Validation("departments.pageSize", "Page size must be less than or equal to 100"));

            return await _departmentReadRepository.GetDepartmentsAsync(command.Search, sortField, parsedSortDirection, command.Page, command.PageSize, cancellationToken);
        }
    }
}
