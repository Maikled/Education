using Contracts.DTOs;
using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Departments.CreateDepartment
{
    internal class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(p => p.Name).MustBeSuccessResult(Name.Validate);
            RuleFor(p => p.Slug).MustBeSuccessResult(Slug.Validate);
        }
    }
}
