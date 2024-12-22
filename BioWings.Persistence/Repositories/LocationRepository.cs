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
        latitude = Math.Round(latitude, 6);
        longitude = Math.Round(longitude, 6);
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(squareRef))
        {
            query = query.Where(l => l.SquareRef == squareRef);
        }
        return query.Where(l => l.Latitude == latitude && l.Longitude == longitude)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Location?> GetByIdWithProvinceAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public IQueryable<Location?> GetUnusedLocationRecord() => _dbSet.Include(x => x.Observations).Where(loc => !loc.Observations.Any());
}

