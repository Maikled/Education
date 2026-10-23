using Contracts.DTOs;
using Core.Extensions;
using Domain.Entities.ValueObjects;
using FluentValidation;

namespace Core.Locations.UpdateLocation
{
    internal class UpdateLocationValidator : AbstractValidator<UpdateLocationDto>
    {
        public UpdateLocationValidator()
        {
            RuleFor(p => p.Name).MustBeSuccessResult(Name.Validate);
            RuleFor(p => p.Address).MustBeSuccessResult(p => Address.Validate(p.Country, p.State, p.City, p.Street, p.BuildingNumber));
        }
    }
}
