using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;

public class LocationRepository(AppDbContext dbContext) : GenericRepository<Location>(dbContext), ILocationRepository
{
    public async Task<Location?> FirstOrDefaultAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<Location?> GetByCoordinatesAsync(decimal latitude, decimal longitude, string squareRef, CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(l =>
            Math.Abs(l.Latitude - latitude) < 0.000001m &&
             Math.Abs(l.Longitude - longitude) < 0.000001m &&
            (string.IsNullOrEmpty(squareRef) || l.SquareRef == squareRef),
            cancellationToken);
    }

    public async Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public IQueryable<Location?> GetUnusedLocationRecord() => _dbSet.Include(x => x.Observations).Where(loc => !loc.Observations.Any());
}

