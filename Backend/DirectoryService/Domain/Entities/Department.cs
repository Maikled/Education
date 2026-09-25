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

        public static Department Create(Name name, Slug slug, DepartmentPath path, Guid? parentId)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Department name cannot be null.");
            }

            if (slug == null)
            {
                throw new ArgumentNullException(nameof(slug), "Department slug cannot be null.");
            }
            
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "Department path cannot be null.");
            }

            if (parentId == Guid.Empty)
            {
                throw new ArgumentException("ParentId cannot be an empty GUID.", nameof(parentId));
            }

            return new Department(name, slug, path, parentId);
        }

        public static Department CreateChild(Department parent, Name name, Slug slug)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent), "Parent department cannot be null.");
            }

            return Create(name, slug, parent.Path.AppendPath(slug), parent.Id);
        }
    }
}
