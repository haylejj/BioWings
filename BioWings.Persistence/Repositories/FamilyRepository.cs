using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class FamilyRepository(AppDbContext dbContext) : GenericRepository<Family>(dbContext), IFamilyRepository
{
    public async Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(f => f.Name == name, cancellationToken);
}

