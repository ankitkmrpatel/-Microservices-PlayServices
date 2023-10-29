using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Data.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
{
    private readonly IRepo<CatalogItem> repo;
    public CatalogItemDeletedConsumer(IRepo<CatalogItem> repo)
    {
        this.repo = repo;
    }

    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var message = context.Message;
        var item = await repo.GetAsync(message.ItemId);

        if (item == null)
        {
            return;
        }

        await repo.DeleteAsync(item.Id);
    }
}
