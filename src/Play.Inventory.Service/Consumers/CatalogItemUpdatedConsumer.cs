using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Data.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
{
    private readonly IRepo<CatalogItem> repo;
    public CatalogItemUpdatedConsumer(IRepo<CatalogItem> repo)
    {
        this.repo = repo;
    }

    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var message = context.Message;
        var item = await repo.GetAsync(message.ItemId);

        if (item == null)
        {
            item = new CatalogItem()
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };

            await repo.CreateAsync(item);
        }
        else
        {
            item.Id = message.ItemId;
            item.Name = message.Name;
            item.Description = message.Description;

            await repo.UpdateAsync(item);
        }
    }
}
