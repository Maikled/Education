using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Domain.Entities.ValueObjects
{
    public record DepartmentPath
    {
        public string Value { get; }

        private DepartmentPath(string value)
        {
            Value = value;
        }

        private static UnitResult<AppError> ValidateSlug(Slug slug)
        {
            if (slug == null)
            {
                return AppError.Validation("departmentPath.slug", "Slug cannot be null.");
            }

            return UnitResult.Success<AppError>();
        }

        public static Result<DepartmentPath, AppError> Create(Slug slug)
        {
            var validateResult = ValidateSlug(slug);
            if (validateResult.IsFailure)
            {
                return validateResult.Error;
            }

            return new DepartmentPath(slug.Value);
        }

        public Result<DepartmentPath, AppError> AppendPath(Slug slug)
        {
            var validateResult = ValidateSlug(slug);
            if (validateResult.IsFailure)
            {
                return validateResult.Error;
            }

            return new DepartmentPath($"{Value}/{slug.Value}");
        }

        public Result<bool, AppError> IsDescendantOf(DepartmentPath ancestor)
        {
            if (ancestor == null)
            {
                return AppError.Validation("departmentPath.ancestor", "Ancestor path cannot be null.");
            }

            return Value.StartsWith($"{ancestor.Value}/", StringComparison.Ordinal);
        }
    }
}
