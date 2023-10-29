using System.ComponentModel.DataAnnotations;

namespace Play.Inventory.Service.Dtos;

public record CatalogItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate)
{
};

public record CreateCatalogItemDto([Required] string Name, string Description, [MinLength(0)] decimal Price)
{
};

public record UpdateCatalogItemDto([Required] string Name, string Description, [MinLength(0)] decimal Price)
{
};
