using Play.Common.Data;

namespace Play.Inventory.Service.Data.Entities
{
    public class CatalogItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
