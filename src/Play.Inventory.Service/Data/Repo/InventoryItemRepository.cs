using MongoDB.Driver;
using Play.Common.Data;
using Play.Inventory.Service.Data.Entities;

namespace Play.Inventory.Service.Data.Repo;

public class InventoryItemRepository : GenericRepository<InventoryItem>
{
    protected override string collectionName { get; set; } = "InventoryItems";

    public InventoryItemRepository(IMongoDatabase db) : base(db)
    {
        
    }
}
