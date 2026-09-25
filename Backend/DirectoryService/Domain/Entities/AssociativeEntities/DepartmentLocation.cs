namespace Domain.Entities.AssociativeEntities
{
    public class DepartmentLocation
    {
        public Guid Id { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid LocationId { get; private set; }
        public bool IsPrimary { get; private set; }

        private DepartmentLocation(Guid departmentId, Guid locationId, bool isPrimary)
        {
            Id = Guid.CreateVersion7();
            DepartmentId = departmentId;
            LocationId = locationId;
            IsPrimary = isPrimary;
        }

        public static DepartmentLocation Create(Guid departmentId, Guid locationId, bool isPrimary)
        {
            if (departmentId == Guid.Empty)
            {
                throw new ArgumentException("DepartmentId cannot be an empty GUID.", nameof(departmentId));
            }
            if (locationId == Guid.Empty)
            {
                throw new ArgumentException("LocationId cannot be an empty GUID.", nameof(locationId));
            }

            return new DepartmentLocation(departmentId, locationId, isPrimary);
        }
    }
}
