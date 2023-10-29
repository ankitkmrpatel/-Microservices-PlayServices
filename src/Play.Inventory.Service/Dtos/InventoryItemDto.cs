using System.ComponentModel.DataAnnotations;

namespace Play.Inventory.Service.Dtos;

public record GrantItemDto(Guid UserId, Guid CatalogItemId, int Quantity)
{
};

public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate)
{
};

public record InventoryItemDetailsDto(Guid CatalogItemId, string Name, string Desctiption, int Quantity, DateTimeOffset AcquiredDate)
{
};
