using System.Linq.Expressions;

namespace Play.Common;

public interface IRepo<T>
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> func);
    Task<T> GetAsync(Guid id);
    Task<T> GetAsync(Expression<Func<T, bool>> func);
    Task CreateAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(Guid id);
}
