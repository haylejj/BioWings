using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

public class SubspeciesRepository(AppDbContext dbContext) : GenericRepository<Subspecies>(dbContext), ISubspeciesRepository
{
    public async Task<IEnumerable<Subspecies>> GetAllWithSpeciesAsync(CancellationToken cancellationToken = default) => await _dbSet.Include(s => s.Species).ToListAsync(cancellationToken);

    public async Task<Subspecies?> GetByIdWithSpeciesAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Species).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Subspecies?> GetByNameAndSpeciesIdAsync(string name, int speciesId, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(x => x.Name == name && x.SpeciesId == speciesId, cancellationToken);
}

