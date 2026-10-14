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
            if (name == null)
            {
                return AppError.Validation("department.name", "Department name cannot be null.");
            }

            if (slug == null)
            {
                return AppError.Validation("department.slug", "Department slug cannot be null.");
            }
            
            if (path == null)
            {
                return AppError.Validation("department.path", "Department path cannot be null.");
            }

            if (parentId == Guid.Empty)
            {
                return AppError.Validation("department.parentId", "ParentId cannot be an empty GUID.");
            }

            return new Department(name, slug, path, parentId);
        }

        public UnitResult<AppError> Update(Name name, Slug slug, DepartmentPath path, Guid? parentId)
        {
            if (name == null)
            {
                return AppError.Validation("department.name", "Department name cannot be null.");
            }

            if (slug == null)
            {
                return AppError.Validation("department.slug", "Department slug cannot be null.");
            }

            if (path == null)
            {
                return AppError.Validation("department.path", "Department path cannot be null.");
            }

            if (parentId == Guid.Empty)
            {
                return AppError.Validation("department.parentId", "ParentId cannot be an empty GUID.");
            }

            Name = name;
            Slug = slug;
            Path = path;
            ParentId = parentId;
            UpdatedAt = DateTime.UtcNow;

            return UnitResult.Success<AppError>();
        }

        public static Result<Department, AppError> CreateChild(Department parent, Name name, Slug slug)
        {
            if (parent == null)
            {
                return AppError.Validation("department.parent", "Parent department cannot be null.");
            }

            var appendPathResult = parent.Path.AppendPath(slug);
            if (appendPathResult.IsFailure)
                return appendPathResult.Error;

            return Create(name, slug, appendPathResult.Value, parent.Id);
        }
    }
}
