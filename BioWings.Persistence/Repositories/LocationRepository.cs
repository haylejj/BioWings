using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;

public class LocationRepository(AppDbContext dbContext) : GenericRepository<Location>(dbContext), ILocationRepository
{
    public async Task<Location?> FirstOrDefaultAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

