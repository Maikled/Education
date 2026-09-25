namespace Domain.Entities.ValueObjects
{
    public record DepartmentPath
    {
        public string Value { get; }

        private DepartmentPath(string value)
        {
            Value = value;
        }

        private static void ValidateSlug(Slug slug)
        {
            if (slug == null)
            {
                throw new ArgumentNullException(nameof(slug), "Slug cannot be null.");
            }
        }

        public static DepartmentPath Create(Slug slug)
        {
            ValidateSlug(slug);

            return new DepartmentPath(slug.Value);
        }

        public DepartmentPath AppendPath(Slug slug)
        {
            ValidateSlug(slug);

            return new DepartmentPath($"{Value}/{slug.Value}");
        }
    }
}
