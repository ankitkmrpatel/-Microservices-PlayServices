using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients;

public class CatalogClinet : ICatalogClinet
{
    private readonly HttpClient client;
    public CatalogClinet(HttpClient client)
    {
        this.client = client;
    }

    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
    {
        var items = await client.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/api/items");
        return items;
    }
}
