using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Positions.CreatePosition
{
    internal class CreatePositionValidator : AbstractValidator<CreatePositionCommand>
    {
        public CreatePositionValidator()
        {
            RuleFor(p => p.CreatePositionDto.Name).MustBeSuccessResult(Name.Validate);
        }
    }
}
