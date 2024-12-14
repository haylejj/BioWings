using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;

public class ObservationRepository(AppDbContext dbContext) : GenericRepository<Observation>(dbContext), IObservationRepository
{
    public async Task<Observation?> FirstOrDefaultAsync(Expression<Func<Observation, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Observation?> GetByIdWithAllNavigationsAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Observer)
            .Include(x => x.Location).ThenInclude(y => y.Province)
            .Include(x => x.Species).ThenInclude(y => y.Genus).ThenInclude(z => z.Family)
            .Include(x => x.Species).ThenInclude(x => x.SpeciesType).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

