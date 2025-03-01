using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Application.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    // Temel CRUD operasyonları
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> GetAllAsNoTracking();
    IQueryable<T> GetAllAsQueryable();
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    //Toplu işlemler
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void UpdateRange(IEnumerable<T> entities);
    void RemoveRange(IEnumerable<T> entities);
    // Sayfalama
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    IQueryable<T> GetPagedAsQueryable(int pageNumber, int pageSize);
    Task<IEnumerable<T>> GetPagedAsNoTrackingAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    // Filtreleme
    Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetFilteredAsNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TResult> ExecuteStoredProcedureAsync<TResult>(string procedureName, object parameters, CancellationToken cancellationToken = default);
    Task ExecuteStoredProcedureAsync(string procedureName, object parameters, CancellationToken cancellationToken = default);
    Task BulkInsertAsync<TResult>(string tableName, IList<TResult> entities, CancellationToken cancellationToken = default);
    Task ExecuteStoredProcedureWithoutNoParamatersAsync(string storedProcedureName, CancellationToken cancellationToken = default);

}
