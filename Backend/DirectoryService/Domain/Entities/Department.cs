using Contracts.Errors;
using CSharpFunctionalExtensions;
using Domain.Entities.ValueObjects;

namespace Domain.Entities
{
    public class Department
    {
        public Guid Id { get; private set; }
        public Name Name { get; private set; }
        public Slug Slug { get; private set; }
        public DepartmentPath Path { get; private set; }
        public Guid? ParentId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Department(Name name, Slug slug, DepartmentPath path, Guid? parentId)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Slug = slug;
            Path = path;
            ParentId = parentId;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Department, AppError> Create(Name name, Slug slug, DepartmentPath path, Guid? parentId)
        {
            var validationResult = Validate(name, slug, path, parentId);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Department>();

            return new Department(name, slug, path, parentId);
        }

        public UnitResult<AppError> Update(Name name, Slug slug, DepartmentPath path, Guid? parentId)
        {
            var validationResult = Validate(name, slug, path, parentId);
            if (validationResult.IsFailure)
                return validationResult;

            Name = name;
            Slug = slug;
            Path = path;
            ParentId = parentId;
            UpdatedAt = DateTime.UtcNow;

            return UnitResult.Success<AppError>();
        }

        public static Result<Department, AppError> CreateChild(Department parent, Name name, Slug slug)
        {
            var validationResult = ValidateChild(parent, name, slug);
            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Department>();

            var appendPathResult = parent.Path.AppendPath(slug);
            if (appendPathResult.IsFailure)
                return appendPathResult.Error;

            return Create(name, slug, appendPathResult.Value, parent.Id);
        }

        public static UnitResult<AppError> Validate(Name name, Slug slug, DepartmentPath path, Guid? parentId)
        {
            return Result.Combine(errors => AppError.FromErrors(errors), ValidateName(name), ValidateSlug(slug), ValidateDepartmentPath(path), ValidateParentId(parentId));
        }

        public static UnitResult<AppError> ValidateChild(Department parent, Name name, Slug slug)
        {
            return Result.Combine(errors => AppError.FromErrors(errors), ValidateName(name), ValidateSlug(slug), ValidateParent(parent));
        }

        private static UnitResult<AppError> ValidateName(Name name)
        {
            return UnitResult.FailureIf(name == null, AppError.Validation("department.name", "Department name cannot be null."));
        }

        private static UnitResult<AppError> ValidateSlug(Slug slug)
        {
            return UnitResult.FailureIf(slug == null, AppError.Validation("department.slug", "Department slug cannot be null."));
        }

        private static UnitResult<AppError> ValidateDepartmentPath(DepartmentPath path)
        {
            return UnitResult.FailureIf(path == null, AppError.Validation("department.path", "Department path cannot be null."));
        }

        private static UnitResult<AppError> ValidateParentId(Guid? parentId)
        {
            return UnitResult.FailureIf(parentId == Guid.Empty, AppError.Validation("department.parentId", "ParentId cannot be an empty GUID."));
        }

        private static UnitResult<AppError> ValidateParent(Department parent)
        {
            return UnitResult.FailureIf(parent == null, AppError.Validation("department.parent", "Parent department cannot be null."));
        }
    }
}
