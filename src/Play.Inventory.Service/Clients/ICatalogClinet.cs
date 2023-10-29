using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public interface ICatalogClinet
    {
        Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync();
    }
}