using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Departments.UpdateDepartment
{
    internal class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(p => p.UpdateDepartmentDto.Name).MustBeSuccessResult(Name.Validate);
            RuleFor(p => p.UpdateDepartmentDto.Slug).MustBeSuccessResult(Slug.Validate);
        }
    }
}
