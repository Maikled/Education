using Contracts.DTOs;
using Contracts.Errors;
using Core.Departments.Interfaces;
using Core.Interfaces;
using CSharpFunctionalExtensions;

namespace Core.Departments.GetDepartment
{
    internal class GetDepartmentHandler : ICommandHandler<DepartmentDto, GetDepartmentCommand>
    {
        private readonly IDepartmentReadRepository _departmentReadRepository;

        public GetDepartmentHandler(IDepartmentReadRepository departmentReadRepository)
        {
            _departmentReadRepository = departmentReadRepository;
        }

        public async Task<Result<DepartmentDto, AppError>> HandlerAsync(GetDepartmentCommand command, CancellationToken cancellationToken)
        {
            var department = await _departmentReadRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
            if (department == null)
            {
                return AppError.NotFound("department", "Department not found");
            }

            return department;
        }
    }
}
