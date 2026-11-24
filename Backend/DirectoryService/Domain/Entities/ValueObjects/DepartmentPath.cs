using Contracts.Errors;
using CSharpFunctionalExtensions;

namespace Domain.Entities.ValueObjects
{
    public record DepartmentPath
    {
        private const char SEPARATOR = '/';

        public string Value { get; }

        private DepartmentPath(string value)
        {
            Value = value;
        }

        public static Result<DepartmentPath, AppError> Create(Slug slug)
        {
            var validationResult = Validate(slug);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<DepartmentPath>();

            return new DepartmentPath(slug.Value);
        }

        /// <summary>
        /// Восстанавливает путь из строки вида "root/child/grandchild" (например, при чтении из БД).
        /// Каждый сегмент должен быть корректным slug.
        /// </summary>
        public static Result<DepartmentPath, AppError> FromValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return AppError.Validation("departmentPath.value", "Path cannot be null or empty.");

            foreach (var segment in value.Split(SEPARATOR))
            {
                var segmentResult = Slug.Create(segment);
                if (segmentResult.IsFailure)
                    return segmentResult.Error;
            }

            return new DepartmentPath(value);
        }

        public Result<DepartmentPath, AppError> AppendPath(Slug slug)
        {
            var validationResult = Validate(slug);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<DepartmentPath>();

            return new DepartmentPath($"{Value}/{slug.Value}");
        }

        public Result<bool, AppError> IsDescendantOf(DepartmentPath ancestor)
        {
            var validationResult = ValidateAncestor(ancestor);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<bool>();

            return Value.StartsWith($"{ancestor.Value}/", StringComparison.Ordinal);
        }

        public static UnitResult<AppError> Validate(Slug slug)
        {
            return ValidateSlug(slug);
        }

        private static UnitResult<AppError> ValidateSlug(Slug slug)
        {
            return UnitResult.FailureIf(slug == null, AppError.Validation("departmentPath.slug", "Slug cannot be null."));
        }

        private static UnitResult<AppError> ValidateAncestor(DepartmentPath ancestor)
        {
            return UnitResult.FailureIf(ancestor == null, AppError.Validation("departmentPath.ancestor", "Ancestor path cannot be null."));
        }
    }
}
