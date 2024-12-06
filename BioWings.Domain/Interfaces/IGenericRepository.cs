using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Domain.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    // Temel CRUD operasyonları
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> GetAllAsNoTracking();
    //Toplu işlemler
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void UpdateRange(IEnumerable<T> entities);
    void RemoveRange(IEnumerable<T> entities);
    // Sayfalama
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetPagedAsNoTrackingAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    // Filtreleme
    Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetFilteredAsNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
