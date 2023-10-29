using MongoDB.Driver;
using Play.Common.Data;
using Play.Catalog.Service.Data.Entities;

namespace Play.Catalog.Service.Data.Repo;

public class ItemRepository : GenericRepository<Item>
{
    protected override string collectionName { get; set; } = "Items";

    public ItemRepository(IMongoDatabase db) : base(db)
    {
        
    }
}
