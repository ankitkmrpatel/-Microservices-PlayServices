using MongoDB.Driver;
using Play.Common.Data;
using Play.Inventory.Service.Data.Entities;

namespace Play.Inventory.Service.Data.Repo;

public class CatalogItemRepository : GenericRepository<CatalogItem>
{
    protected override string collectionName { get; set; } = "CatalogItems";

    public CatalogItemRepository(IMongoDatabase db) : base(db)
    {
        
    }
}
