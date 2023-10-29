using Play.Common.Data;

namespace Play.Inventory.Service.Data.Entities;

public class InventoryItem : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CatalogItemId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset AcquiredDate { get; set; }
}
