namespace Play.Common.Data
{
    public class BaseEntity : IMustHaveId
    {
        public Guid Id { get; set; }
    }
}
