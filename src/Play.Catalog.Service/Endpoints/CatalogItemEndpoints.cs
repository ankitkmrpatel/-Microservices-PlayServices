using Play.Common;
using Play.Catalog.Service.Data.Entities;
using Play.Catalog.Service.Extentions;
using Play.Catalog.Service.Dtos;
using MassTransit;
using Play.Catalog.Contracts;
using MassTransit.RabbitMqTransport;
using SharpCompress.Common;

namespace Play.Catalog.Service.Endpoints
{
    public static class CatalogItemEndpoints
    {
        private static int requestGetCounter = 0;

        internal static void MapCatalogItemEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/items", GetAllItems)
                .WithName("GetAllItems").WithDisplayName("Get All Catalog Items")
                .Produces<IReadOnlyCollection<CatalogItemDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            app.MapGet("api/items/{id}", GetItemById)
                .WithName("GetItemById").WithDisplayName("Get Catalog Item")
                .Produces<CatalogItemDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            app.MapPost("api/items", CreateNewItems)
                .WithName("CreateNewItems").WithDisplayName("Create New Catalog Item")
                .Produces<CatalogItemDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            app.MapPut("api/items", UpdateItem)
                .WithName("UpdateItem").WithDisplayName("Update Catalog Item")
                .Produces<CatalogItemDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            app.MapDelete("api/items/{id}", DeleteItem)
                .WithName("DeleteItem")
                .WithDisplayName("Delete Catalog Item")
                .Produces<CatalogItemDto>(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Get All Catalog Items
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        private static async Task<IResult> GetAllItems(IRepo<Item> repo)
        {
            var entities = await repo.GetAllAsync();
            var items = entities.Select(x => x.AsDto());

            return Results.Ok(items);
        }

        /// <summary>
        /// Get the Catalog Item
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static async Task<IResult> GetItemById(IRepo<Item> repo, Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return Results.BadRequest();
            }

            var entity = await repo.GetAsync(id);

            if (null == entity)
                return Results.NotFound();

            var item = entity.AsDto();
            return Results.Ok(item);
        }

        /// <summary>
        /// Create New Catalog Item
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        private static async Task<IResult> CreateNewItems(IRepo<Item> repo, IPublishEndpoint publisher, CreateCatalogItemDto newItem)
        {
            if (string.IsNullOrEmpty(newItem.Name))
                return Results.BadRequest();

            if (string.IsNullOrEmpty(newItem.Description))
                return Results.BadRequest();

            if (newItem.Price < 0)
                return Results.BadRequest();

            var entity = new Item()
            {
                Name = newItem.Name,
                Description = newItem.Description,
                Price = newItem.Price,
                CreatedTime = DateTimeOffset.UtcNow
            };

            await repo.CreateAsync(entity);
            var item = entity.AsDto();

            await publisher.Publish(new CatalogItemCreated(entity.Id, entity.Name, entity.Description));

            return Results.Created($"api/items/{entity.Id}", item);
        }

        /// <summary>
        /// Update Catalog Item
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id"></param>
        /// <param name="updateItem"></param>
        /// <returns></returns>
        private static async Task<IResult> UpdateItem(IRepo<Item> repo, IPublishEndpoint publisher, Guid id, UpdateCatalogItemDto updateItem)
        {
            if (id.Equals(Guid.Empty))
            {
                return Results.BadRequest();
            }

            var existingItem = await repo.GetAsync(id);

            if (null == existingItem)
                return Results.NotFound();

            existingItem.Name = updateItem.Name;
            existingItem.Description = updateItem.Description;
            existingItem.Price = updateItem.Price;
            await repo.UpdateAsync(existingItem);

            await publisher.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return Results.Ok(existingItem);
        }

        /// <summary>
        /// Delete The Catalog Item
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static async Task<IResult> DeleteItem(IRepo<Item> repo, IPublishEndpoint publisher, Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return Results.BadRequest();
            }

            var existingItem = await repo.GetAsync(id);

            if (null == existingItem)
                return Results.NotFound();

            await repo.DeleteAsync(existingItem.Id);

            await publisher.Publish(new CatalogItemDeleted(existingItem.Id));

            return Results.NoContent();
        }
    }
}
