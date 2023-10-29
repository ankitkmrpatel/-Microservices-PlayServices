using Play.Common.Data;

namespace Play.Catalog.Service.Data.Entities
{
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
