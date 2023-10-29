using Play.Catalog.Service.Data.Entities;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Extentions;

public static class Extention
{
    public static CatalogItemDto AsDto(this Item item)
    {
        return new CatalogItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedTime);
    }
}
