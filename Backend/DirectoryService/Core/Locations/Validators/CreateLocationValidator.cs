using Contracts.DTOs;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Locations.Validators
{
    internal sealed class CreateLocationValidator : AbstractValidator<CreateLocationDto>
    {
        public CreateLocationValidator()
        {
            RuleFor(p => p.Name).NotNull().NotEmpty().WithMessage("Name is required.").MaximumLength(Name.MAX_LENGTH).WithMessage($"Name length cannot be more than {Name.MAX_LENGTH} characters.").MinimumLength(Name.MIN_LENGTH).WithMessage($"Name length cannot be less than {Name.MIN_LENGTH} characters.");
            RuleFor(p => p.Country).NotNull().NotEmpty().WithMessage("Country is required.");
            RuleFor(p => p.State).NotNull().NotEmpty().WithMessage("State is required.");
            RuleFor(p => p.City).NotNull().NotEmpty().WithMessage("City is required.");
            RuleFor(p => p.Street).NotNull().NotEmpty().WithMessage("Street is required.");
            RuleFor(p => p.BuildingNumber).NotNull().NotEmpty().WithMessage("Building number is required.");
        }
    }
}
