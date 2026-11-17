using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Positions.UpdatePosition
{
    internal class UpdatePositionValidator : AbstractValidator<UpdatePositionCommand>
    {
        public UpdatePositionValidator()
        {
            RuleFor(p => p.UpdatePositionDto.Name).MustBeSuccessResult(Name.Validate);
        }
    }
}
