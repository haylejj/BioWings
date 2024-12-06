using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.UnitOfWork;
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
    public int SaveChanges() => context.SaveChanges();
    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
