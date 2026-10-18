using Contracts.Errors;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Extensions
{
    internal static class CustomValidateExtension
    {
        public static IRuleBuilderOptionsConditions<T, TProperty> MustBeSuccessResult<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, UnitResult<AppError>> factory)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                var result = factory(value);
                if (result.IsSuccess)
                    return;

                context.AddFailure(new ValidationFailure(context.PropertyPath, result.Error.Message)
                {
                    ErrorCode = result.Error.Code
                });
            });
        }
    }
}
