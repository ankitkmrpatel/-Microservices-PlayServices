using Play.Inventory.Service.Data.Entities;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Extentions;

public static class Extention
{
    public static InventoryItemDto AsDto(this InventoryItem item)
    {
        return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
    }
    public static InventoryItemDetailsDto AsDto(this InventoryItem item, string name, string desc)
    {
        return new InventoryItemDetailsDto(item.CatalogItemId, name, desc, item.Quantity, item.AcquiredDate);
    }
}
