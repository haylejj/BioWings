using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class SpeciesTypeRepository(AppDbContext dbContext) : GenericRepository<SpeciesType>(dbContext), ISpeciesTypeRepository
{
    public async Task<SpeciesType?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
}

