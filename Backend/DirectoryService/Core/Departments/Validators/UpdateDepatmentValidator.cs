using Contracts.DTOs;
using FluentValidation;

namespace Core.Departments.Validators
{
    internal class UpdateDepatmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepatmentValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100).WithMessage($"Name length cannot be more than 100 characters.").MinimumLength(3).WithMessage($"Name length cannot be less than 3 characters.");
            RuleFor(p => p.Slug).NotEmpty().WithMessage("Slug is required.").MaximumLength(100).WithMessage($"Slug length cannot be more than 100 characters.").MinimumLength(3).WithMessage($"Slug length cannot be less than 3 characters.");
            RuleFor(p => p.ParentId).NotEqual(default(Guid)).WithMessage("ParentId is not be default value.");
        }
    }
}
