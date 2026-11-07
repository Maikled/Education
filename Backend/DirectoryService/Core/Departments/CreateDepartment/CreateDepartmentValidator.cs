using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Departments.CreateDepartment
{
    internal class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(p => p.CreateDepartmentDto.Name).MustBeSuccessResult(Name.Validate);
            RuleFor(p => p.CreateDepartmentDto.Slug).MustBeSuccessResult(Slug.Validate);
        }
    }
}
