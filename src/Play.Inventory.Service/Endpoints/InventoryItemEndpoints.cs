using Play.Common;
using Play.Inventory.Service.Data.Entities;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Extentions;

namespace Play.Inventory.Service.Endpoints
{
    public static class InventoryItemEndpoints
    {
        internal static void MapInventoryItemEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/intentories/{userId}", GetAllItems)
                .WithName("GetAllItems").WithDisplayName("Get All Catalog Items")
                .Produces<IReadOnlyCollection<InventoryItemDetailsDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            //app.MapGet("api/intentories/{userId}/{id}", GetItemById)
            //    .WithName("GetItemById")
            //    .WithDisplayName("Get Catalog Item");

            app.MapPost("api/intentories", CreateUpdateInventoryItems)
                .WithName("CreateNewItems")
                .WithDisplayName("Create New Catalog Item")
                .Produces<InventoryItemDetailsDto>(StatusCodes.Status201Created)
                .Produces<InventoryItemDetailsDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            //app.MapPut("api/items", UpdateItem)
            //    .WithName("UpdateItem")
            //    .WithDisplayName("Update Catalog Item");

            //app.MapDelete("api/items/{id}", DeleteItem)
            //    .WithName("DeleteItem")
            //    .WithDisplayName("Delete Catalog Item");
        }

        /// <summary>
        /// Get All Catalog Items
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        private static async Task<IResult> GetAllItems(IRepo<CatalogItem> catalogRepo, IRepo<InventoryItem> itemRepo, Guid userId)
        {
            if (userId.Equals(Guid.Empty))
            {
                return Results.BadRequest();
            }

            var inventoryItems = await itemRepo.GetAllAsync(item => item.UserId.Equals(userId));
            var allCatalogItemIds = inventoryItems.Select(x => x.CatalogItemId);

            var catalogItems = await catalogRepo.GetAllAsync(x => allCatalogItemIds.Contains(x.Id));

            var inventoryItemDetailsDto = inventoryItems.Select(x => 
            {
                var catalog = catalogItems.Single(y => y.Id.Equals(x.CatalogItemId));
                return x.AsDto(catalog.Name, catalog.Description);
            });

            return Results.Ok(inventoryItemDetailsDto);
        }

        ///// <summary>
        ///// Get the Catalog Item
        ///// </summary>
        ///// <param name="repo"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //private static async Task<IResult> GetItemById(IRepo<InventoryItem> repo, Guid userId, Guid id)
        //{
        //    if (userId.Equals(Guid.Empty))
        //    {
        //        return Results.BadRequest();
        //    }

        //    if (id.Equals(Guid.Empty))
        //    {
        //        return Results.BadRequest();
        //    }

        //    var entity = await repo.GetAsync(item => item.Id.Equals(id) && 
        //        item.UserId.Equals(userId));

        //    if (null == entity)
        //        return Results.NotFound();

        //    var item = entity.AsDto();
        //    return Results.Ok(item);
        //}

        /// <summary>
        /// Create New Catalog Item
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="grantItem"></param>
        /// <returns></returns>
        private static async Task<IResult> CreateUpdateInventoryItems(IRepo<InventoryItem> repo, GrantItemDto grantItem)
        {
            if (null == grantItem)
                return Results.BadRequest();

            if (grantItem.UserId.Equals(Guid.Empty))
                return Results.BadRequest();

            if (grantItem.CatalogItemId.Equals(Guid.Empty))
                return Results.BadRequest();

            if (grantItem.Quantity < 0)
                return Results.BadRequest();

            var inventoryItem = await repo.GetAsync(item => item.UserId.Equals(grantItem.UserId) 
                && item.CatalogItemId.Equals(grantItem.CatalogItemId));

            if (null == inventoryItem)
            {
                var entity = new InventoryItem()
                {
                    UserId = grantItem.UserId,
                    CatalogItemId = grantItem.CatalogItemId,
                    Quantity = grantItem.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await repo.CreateAsync(entity);

                var inventoryDetailItem = entity.AsDto();

                return Results.Created($"api/intentories/{entity.UserId}/{entity.Id}", inventoryDetailItem);
            }

            inventoryItem.Quantity += grantItem.Quantity;
            await repo.UpdateAsync(inventoryItem);

            var inventoryDetailedItem = inventoryItem.AsDto();
            return Results.Ok(inventoryDetailedItem);
        }

        ///// <summary>
        ///// Update Catalog Item
        ///// </summary>
        ///// <param name="repo"></param>
        ///// <param name="id"></param>
        ///// <param name="updateItem"></param>
        ///// <returns></returns>
        //private static async Task<IResult> UpdateItem(IRepo<InventoryItem> repo, Guid id, UpdateCatalogItemDto updateItem)
        //{
        //    var existingItem = await repo.GetAsync(id);

        //    if (null == existingItem)
        //        return Results.NotFound();

        //    existingItem.Name = updateItem.Name;
        //    existingItem.Description = updateItem.Description;
        //    existingItem.Price = updateItem.Price;

        //    await repo.UpdateAsync(existingItem);

        //    return Results.Ok(existingItem);
        //}

        ///// <summary>
        ///// Delete The Catalog Item
        ///// </summary>
        ///// <param name="repo"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //private static async Task<IResult> DeleteItem(IRepo<InventoryItem> repo, Guid id)
        //{
        //    var existingItem = await repo.GetAsync(id);

        //    if (null == existingItem)
        //        return Results.NotFound();

        //    await repo.DeleteAsync(existingItem.Id);

        //    return Results.NoContent();
        //}
    }
}
