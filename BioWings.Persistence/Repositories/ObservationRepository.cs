using BioWings.Application.DTOs.ExportDtos;
using BioWings.Application.DTOs.StatisticsDtos;
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
    public async Task RemoveDuplicatesWithProcedureAsync(string procedureName, CancellationToken cancellationToken = default)
    {
        await ExecuteStoredProcedureWithoutNoParamatersAsync(procedureName, cancellationToken);
    }
    public async Task RemoveDuplicateObservationsWithEfCoreAsync(CancellationToken cancellationToken)
    {
        const int batchSize = 1500;

        var duplicateGroups = await _dbSet
            .GroupBy(o => new { o.SpeciesId, o.LocationId, o.ObserverId, o.ObservationDate, o.LocationInfo, o.Notes, o.Source })
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.OrderBy(x => x.Id).Skip(1))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        for (int i = 0; i < duplicateGroups.Count; i += batchSize)
        {
            var batch = await _dbSet.Where(x => duplicateGroups.Skip(i).Take(batchSize).Contains(x.Id))
                .ToListAsync(cancellationToken);
            _dbSet.RemoveRange(batch);
            await dbContext.SaveChangesAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
        }
    }
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default) => await _dbSet.CountAsync(cancellationToken);

    public IQueryable<Observation> GetObservationsForExporting(DateTime? startDate, DateTime? endDate, int? recordLimit, bool exportAllDates, bool exportAllRecords, List<ExpertColumnInfo> columns)
    {
        var query = _dbSet.AsNoTracking();

        // Tarihe göre filtreleme
        if (!exportAllRecords)
        {
            if (!exportAllDates)
            {
                if (startDate.HasValue)
                    query = query.Where(x => x.ObservationDate >= startDate);
                if (endDate.HasValue)
                    query = query.Where(x => x.ObservationDate <= endDate);
            }

            if (recordLimit.HasValue)
                query = query.Take(recordLimit.Value);
        }

        // Include edilecek navigation property'leri belirleme
        var includes = new HashSet<string>();

        foreach (var column in columns)
        {
            if (column.TableName.ToLower() == "species")
            {
                // Önce Species'i ekle
                includes.Add("Species");

                // Genus ve Family için kontrol
                if (column.PropertyPath.Contains("Genus"))
                {
                    includes.Add("Species.Genus");
                    if (column.PropertyPath.Contains("Family"))
                    {
                        includes.Add("Species.Genus.Family");
                    }
                }

                // Authority için kontrol
                if (column.PropertyPath.Contains("Authority"))
                {
                    includes.Add("Species.Authority");
                }
            }
            else if (column.TableName.ToLower() == "location")
            {
                includes.Add("Location");
                if (column.PropertyPath.Contains("Province"))
                {
                    includes.Add("Location.Province");
                }
            }
            else if (column.TableName.ToLower() == "observer")
            {
                includes.Add("Observer");
            }
        }

        // Include'ları hiyerarşik sırayla ekle
        var orderedIncludes = includes.OrderBy(x => x.Count(c => c == '.')).ToList();
        foreach (var include in orderedIncludes)
        {
            query = query.Include(include);
        }

        return query;
    }
    public IQueryable<Observation> GetObservationsByProvinceIdForExporting(int provinceId)
    {
        var query = _dbSet.AsNoTracking().Include(x => x.Location).AsQueryable();
        query = query.Where(x => x.Location.ProvinceId == provinceId);

        query=query.Include(x => x.Species).ThenInclude(x => x.Genus).ThenInclude(x => x.Family);
        query=query.Include(x => x.Species).ThenInclude(x => x.Authority);
        query=query.Include(x => x.Observer);
        query=query.Include(x => x.Location).ThenInclude(x => x.Province);

        return query;
    }

    public async Task<List<ProvinceStatDto>> GetTopProvincesAsync(CancellationToken cancellationToken = default) => await _dbSet.AsNoTracking()
            .Include(x => x.Location)
            .ThenInclude(x => x.Province)
            .GroupBy(x => x.Location.Province)
            .Select(x => new ProvinceStatDto
            {
                ProvinceName = x.Key.Name,
                ObservationCount = x.Count(),
                UniqueSpeciesCount = x.Select(y => y.SpeciesId).Distinct().Count()
            })
            .OrderByDescending(x => x.ObservationCount)
            .Take(5)
            .ToListAsync(cancellationToken);

    public async  Task<(List<Observation>, int)> GetFilteredAsync(List<string> columns, List<string> filterValues, int pageNumber = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        IQueryable<Observation> query = dbContext.Observations
       .Include(o => o.Species)
           .ThenInclude(s => s.Genus)
               .ThenInclude(g => g.Family)
       .Include(o => o.Species)
           .ThenInclude(s => s.Authority)
       .Include(o => o.Location)
           .ThenInclude(l => l.Province)
       .Include(o => o.Observer)
       .AsNoTracking();

        if (columns != null && filterValues != null && columns.Count == filterValues.Count)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                string column = columns[i];
                string filterValue = filterValues[i];

                if (string.IsNullOrWhiteSpace(column) || string.IsNullOrWhiteSpace(filterValue))
                    continue;

                filterValue = filterValue.Trim().ToLower();

                query = column.ToLower() switch
                {
                    "provincename" => query.Where(o => o.Location.Province.Name.ToLower().Contains(filterValue)),
                    "genusname" => query.Where(o => o.Species.Genus.Name.ToLower().Contains(filterValue)),
                    "familyname" => query.Where(o => o.Species.Genus.Family.Name.ToLower().Contains(filterValue)),
                    "scientificname" => query.Where(o => o.Species.ScientificName.ToLower().Contains(filterValue)),
                    "hesselbarthname" => query.Where(o => o.Species.HesselbarthName.ToLower().Contains(filterValue)),
                    "numberseen" => query.Where(o => o.NumberSeen == int.Parse(filterValue)),
                    "observationdate" => DateTime.TryParse(filterValue, out DateTime date)
                       ? query.Where(o => o.ObservationDate.Date == date.Date)
                       : query,
                    _ => query

                };
            }
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var observations = await query
            .OrderByDescending(o => o.ObservationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (observations, totalCount);
    }
}