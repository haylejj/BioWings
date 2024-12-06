using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;
public class GenericRepository<T>(AppDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> _dbSet = dbContext.Set<T>();
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await _dbSet.AddAsync(entity, cancellationToken);
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) => await _dbSet.AddRangeAsync(entities, cancellationToken);
    public IQueryable<T> GetAllAsNoTracking() => _dbSet.AsNoTracking();
    public IQueryable<T> GetAll() => _dbSet;
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.FindAsync(id, cancellationToken);
    public async Task<IEnumerable<T>> GetFilteredAsNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    public async Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    public async Task<IEnumerable<T>> GetPagedAsNoTrackingAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    public void Remove(T entity) => _dbSet.Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
    public void Update(T entity) => _dbSet.Update(entity);
    public void UpdateRange(IEnumerable<T> entities) => _dbSet.UpdateRange(entities);
}
