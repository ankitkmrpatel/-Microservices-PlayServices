using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace Play.Common.Data;

public abstract class GenericRepository<T> : IRepo<T> where T : IMustHaveId
{
    protected abstract string collectionName { get; set; }

    protected readonly IMongoCollection<T> dbCollction;
    protected readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

    public GenericRepository(IMongoDatabase db)
    {
        dbCollction = db.GetCollection<T>(collectionName);
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await dbCollction.Find(filterBuilder.Empty)
            .ToListAsync();
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await dbCollction.Find(filter)
            .ToListAsync();
    }

    public virtual async Task<T> GetAsync(Guid id)
    {
        var filter = filterBuilder.Eq(x => x.Id, id);
        return await dbCollction.Find(filter)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await dbCollction.Find(filter)
            .SingleOrDefaultAsync();
    }

    public virtual async Task CreateAsync(T item)
    {
        if (null == item)
            throw new ArgumentNullException(nameof(item));

        await dbCollction.InsertOneAsync(item);
    }

    public virtual async Task UpdateAsync(T item)
    {
        if (null == item)
            throw new ArgumentNullException(nameof(item));

        var filter = filterBuilder.Eq(x => x.Id, item.Id);
        await dbCollction.ReplaceOneAsync(filter, item);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        if (Guid.Empty.Equals(id))
            throw new ArgumentNullException(nameof(id));

        var filter = filterBuilder.Eq(x => x.Id, id);
        await dbCollction.DeleteOneAsync(filter);
    }
}
