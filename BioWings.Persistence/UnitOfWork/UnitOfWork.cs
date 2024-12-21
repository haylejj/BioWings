using BioWings.Application.Services;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BioWings.Persistence.UnitOfWork;
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IDbContextTransaction _transaction;
    public DbContext GetContext() => context;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction?.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
        GC.SuppressFinalize(this);
    }

    public int SaveChanges() => context.SaveChanges();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
