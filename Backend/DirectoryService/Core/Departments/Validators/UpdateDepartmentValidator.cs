using Contracts.DTOs;
using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Departments.Validators
{
    internal class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(p => p.Name).MustBeSuccessResult(Name.Validate);
            RuleFor(p => p.Slug).MustBeSuccessResult(Slug.Validate);
        }
    }
}
