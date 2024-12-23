using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;

public class ObservationRepository(AppDbContext dbContext) : GenericRepository<Observation>(dbContext), IObservationRepository
{
    public async Task<Observation?> FirstOrDefaultAsync(Expression<Func<Observation, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<List<Observation>> GetAllDuplicateObservationsAsync(CancellationToken cancellationToken = default)
    {
        var datas = await _dbSet
             .FromSqlRaw(@"
            SELECT o.* FROM Observations o
            INNER JOIN (
                SELECT SpeciesId, LocationId, ObserverId, ObservationDate, 
                       LocationInfo, Notes, Source
                FROM Observations
                GROUP BY SpeciesId, LocationId, ObserverId, ObservationDate, 
                         LocationInfo, Notes, Source
                HAVING COUNT(*) > 1
            ) AS dups 
            ON o.SpeciesId = dups.SpeciesId
            AND o.LocationId = dups.LocationId
            AND ((o.ObserverId IS NULL AND dups.ObserverId IS NULL) OR o.ObserverId = dups.ObserverId)
            AND o.ObservationDate = dups.ObservationDate
            AND ((o.LocationInfo IS NULL AND dups.LocationInfo IS NULL) OR o.LocationInfo = dups.LocationInfo)
            AND ((o.Notes IS NULL AND dups.Notes IS NULL) OR o.Notes = dups.Notes)
            AND ((o.Source IS NULL AND dups.Source IS NULL) OR o.Source = dups.Source)
            WHERE o.Id NOT IN (
                SELECT MIN(Id)
                FROM Observations
                GROUP BY SpeciesId, LocationId, ObserverId, ObservationDate, 
                         LocationInfo, Notes, Source
                HAVING COUNT(*) > 1
            )")
             .AsNoTracking()
             .ToListAsync(cancellationToken);
        return datas;
    }
    public async Task RemoveDuplicatesAsync(List<int> batchIds, CancellationToken cancellationToken = default)
    {
        var observationsToDelete = await _dbSet
        .Where(o => batchIds.Contains(o.Id))
        .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(observationsToDelete);
    }

    public async Task<Observation?> GetByIdWithAllNavigationsAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Observer)
            .Include(x => x.Location).ThenInclude(y => y.Province)
            .Include(x => x.Species).ThenInclude(y => y.Genus).ThenInclude(z => z.Family)
            .Include(x => x.Species).ThenInclude(y => y.Authority)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<int>> GetAllDuplicateObservationIdsAsync(CancellationToken cancellationToken = default)
    {
        var duplicateIds = await _dbSet
            .FromSqlRaw(@"
               SELECT o.Id FROM Observations o
               INNER JOIN (
                   SELECT SpeciesId, LocationId, ObserverId, ObservationDate,
                          LocationInfo, Notes, Source, NumberSeen 
                   FROM Observations
                   GROUP BY SpeciesId, LocationId, ObserverId, ObservationDate,
                            LocationInfo, Notes, Source, NumberSeen
                   HAVING COUNT(*) > 1
               ) AS dups 
               ON o.SpeciesId = dups.SpeciesId
               AND o.LocationId = dups.LocationId
               AND ((o.ObserverId IS NULL AND dups.ObserverId IS NULL) OR o.ObserverId = dups.ObserverId)
               AND o.ObservationDate = dups.ObservationDate
               AND ((o.LocationInfo IS NULL AND dups.LocationInfo IS NULL) OR o.LocationInfo = dups.LocationInfo)
               AND ((o.Notes IS NULL AND dups.Notes IS NULL) OR o.Notes = dups.Notes)
               AND ((o.Source IS NULL AND dups.Source IS NULL) OR o.Source = dups.Source)
               AND o.NumberSeen = dups.NumberSeen
               WHERE o.Id NOT IN (
                   SELECT MIN(Id)
                   FROM Observations
                   GROUP BY SpeciesId, LocationId, ObserverId, ObservationDate,
                            LocationInfo, Notes, Source, NumberSeen
                   HAVING COUNT(*) > 1
               )")
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);
        return duplicateIds;
    }
    public async Task BulkInsertObservationsAsync(IList<Observation> observations, CancellationToken cancellationToken = default)
    {
        await BulkInsertAsync("observations", observations, cancellationToken);
    }
}