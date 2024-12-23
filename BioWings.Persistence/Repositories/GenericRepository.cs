using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Collections;
using System.Data;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;
public class GenericRepository<T>(AppDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> _dbSet = dbContext.Set<T>();
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await _dbSet.AddAsync(entity, cancellationToken);
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) => await _dbSet.AddRangeAsync(entities, cancellationToken);
    public IQueryable<T> GetAllAsNoTracking() => _dbSet.AsNoTracking();
    public IQueryable<T> GetAllAsQueryable() => _dbSet.AsQueryable();
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbSet.ToListAsync(cancellationToken);
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.FindAsync(id, cancellationToken);
    public async Task<IEnumerable<T>> GetFilteredAsNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    public async Task<IEnumerable<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    public async Task<IEnumerable<T>> GetPagedAsNoTrackingAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    public void Remove(T entity) => _dbSet.Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
    public void Update(T entity) => _dbSet.Update(entity);
    public void UpdateRange(IEnumerable<T> entities) => _dbSet.UpdateRange(entities);
    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default) => await _dbSet.CountAsync(cancellationToken);
    public IQueryable<T> GetPagedAsQueryable(int pageNumber, int pageSize) => _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize);

    public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(string procedureName, object parameters, CancellationToken cancellationToken = default)
    {
        var paramList = new List<MySqlParameter>();
        foreach (var prop in parameters.GetType().GetProperties())
        {
            paramList.Add(new MySqlParameter($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value));
        }

        var result = await dbContext.Database
        .SqlQueryRaw<TResult>($"CALL {procedureName}({string.Join(",", paramList.Select(p => p.ParameterName))})", paramList.ToArray())
        .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task ExecuteStoredProcedureAsync(string procedureName, object parameters, CancellationToken cancellationToken = default)
    {
        var paramList = new List<MySqlParameter>();
        foreach (var prop in parameters.GetType().GetProperties())
        {
            paramList.Add(new MySqlParameter($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value));
        }

        await dbContext.Database.ExecuteSqlRawAsync($"CALL {procedureName}({string.Join(",", paramList.Select(p => p.ParameterName))})",
                paramList.ToArray(),
                cancellationToken);
    }

    public async Task BulkInsertAsync<TResult>(string tableName, IList<TResult> entities, CancellationToken cancellationToken = default)
    {
        var transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction() as MySqlTransaction;
        var connection = dbContext.Database.GetDbConnection() as MySqlConnection;

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var bulkCopy = new MySqlBulkCopy(connection, transaction)  // transaction'ı burada ver
        {
            DestinationTableName = tableName,
            BulkCopyTimeout = 600
        };

        var dataTable = CreateDataTable(entities);
        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }
    private DataTable CreateDataTable<TResult>(IList<TResult> entities)
    {
        var dataTable = new DataTable();
        var allProperties = typeof(TResult).GetProperties();
        var properties = typeof(TResult)
            .GetProperties()
            .Where(p =>
            {
                // Virtual kontrolü
                var isVirtual = p.GetGetMethod()?.IsVirtual ?? false;
                // Collection kontrolü - daha spesifik
                var isCollection = p.PropertyType != typeof(string) &&
                                 typeof(IEnumerable).IsAssignableFrom(p.PropertyType);
                // Type kontrolü
                var isValidType = p.PropertyType.IsPrimitive ||
                                p.PropertyType == typeof(string) ||
                                p.PropertyType == typeof(DateTime) ||
                                p.PropertyType == typeof(decimal) ||
                                p.PropertyType == typeof(int) ||
                                p.PropertyType == typeof(long) ||
                                (Nullable.GetUnderlyingType(p.PropertyType)?.IsPrimitive ?? false) ||
                                Nullable.GetUnderlyingType(p.PropertyType) == typeof(decimal);
                return !isVirtual && !isCollection && isValidType;
            })
            .ToList();

        foreach (var property in properties)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            dataTable.Columns.Add(property.Name, type);
        }

        // BaseEntity alanları için kolon ekle
        if (!dataTable.Columns.Contains("CreatedDateTime"))
        {
            dataTable.Columns.Add("CreatedDateTime", typeof(DateTime));
        }
        if (!dataTable.Columns.Contains("UpdatedDateTime"))
        {
            dataTable.Columns.Add("UpdatedDateTime", typeof(DateTime));
        }

        foreach (var entity in entities)
        {
            var row = dataTable.NewRow();
            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                row[property.Name] = value ?? DBNull.Value;
            }

            // BaseEntity alanlarını set et
            var now = DateTime.Now;
            row["CreatedDateTime"] = now;
            row["UpdatedDateTime"] = DateTime.MinValue;

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
}
