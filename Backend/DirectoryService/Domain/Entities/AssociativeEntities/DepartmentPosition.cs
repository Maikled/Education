namespace Domain.Entities.AssociativeEntities
{
    public class DepartmentPosition
    {
        public Guid Id { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid PositionId { get; private set; }

        private DepartmentPosition(Guid departmentId, Guid positionId)
        {
            Id = Guid.CreateVersion7();
            DepartmentId = departmentId;
            PositionId = positionId;
        }

        public static DepartmentPosition Create(Guid departmentId, Guid positionId)
        {
            if (departmentId == Guid.Empty)
            {
                throw new ArgumentException("DepartmentId cannot be an empty GUID.", nameof(departmentId));
            }
            if (positionId == Guid.Empty)
            {
                throw new ArgumentException("PositionId cannot be an empty GUID.", nameof(positionId));
            }

            return new DepartmentPosition(departmentId, positionId);
        }
    }
}
