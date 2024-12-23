using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class FamilyRepository(AppDbContext dbContext) : GenericRepository<Family>(dbContext), IFamilyRepository
{
    public async Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking().Where(x => x.Name==name).FirstOrDefaultAsync(cancellationToken);
}

