using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BioWings.Persistence.Interceptors;
public class CustomSaveChangeInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        foreach (var entry in eventData.Context!.ChangeTracker.Entries().AsEnumerable())
        {
            if (entry.Entity is not BaseEntity baseEntity) continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    baseEntity.CreatedDateTime = DateTime.Now;
                    eventData.Context.Entry(baseEntity).Property(x => x.UpdatedDateTime).IsModified = false;
                    break;
                case EntityState.Modified:
                    baseEntity.UpdatedDateTime = DateTime.Now;
                    eventData.Context.Entry(baseEntity).Property(x => x.CreatedDateTime).IsModified = false;
                    break;
            }
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
