using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;
public class SpeciesRepository(AppDbContext dbContext) : GenericRepository<Species>(dbContext), ISpeciesRepository
{
    public async Task<Species?> GetByIdWithGenus_Authority_SpeciesTypeAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(s => s.Genus).Include(s => s.Authority).Include(s => s.SpeciesType).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}

