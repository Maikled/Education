using Contracts.DTOs;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Departments.Validators
{
    internal class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(Name.MAX_LENGTH).WithMessage($"Name length cannot be more than {Name.MAX_LENGTH} characters.").MinimumLength(Name.MIN_LENGTH).WithMessage($"Name length cannot be less than {Name.MIN_LENGTH} characters.");
            RuleFor(p => p.Slug).NotEmpty().WithMessage("Slug is required.").MaximumLength(Slug.MAX_LENGTH).WithMessage($"Slug length cannot be more than {Slug.MAX_LENGTH} characters.").MinimumLength(Slug.MIN_LENGTH).WithMessage($"Slug length cannot be less than {Slug.MIN_LENGTH} characters.");
            RuleFor(p => p.ParentId).NotEqual(default(Guid)).WithMessage("ParentId is not be default value.");
        }
    }
}
