using BioWings.Application.DTOs.StatisticsDtos;
using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;
public class SpeciesRepository(AppDbContext dbContext, IUnitOfWork unitOfWork, ILogger<SpeciesRepository> logger) : GenericRepository<Species>(dbContext), ISpeciesRepository
{
    private readonly DbContext dbContext1 = unitOfWork.GetContext();
    public IQueryable<Species?> GetUnusedSpeciesRecord() => _dbSet.Include(x => x.Observations).Include(x => x.Subspecies).Where(spec => !spec.Observations.Any() &&  !spec.Subspecies.Any());
    public async Task<Species?> FirstOrDefaultAsync(Expression<Func<Species, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Authority).Include(y => y.Genus).FirstOrDefaultAsync(predicate, cancellationToken);
    public async Task<Species?> GetByHesselbarthNameAsync(string hesselbarthName, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(x => x.HesselbarthName== hesselbarthName, cancellationToken);
    public async Task<Species?> GetByIdWithGenus_Authority_SpeciesTypeAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(s => s.Genus).ThenInclude(x => x.Family).Include(s => s.Authority).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Species?> GetByKocakNameAsync(string kocakName, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.KocakName == kocakName, cancellationToken);

    public async Task<Species?> GetByScientificNameAsync(string scientificName, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.ScientificName == scientificName, cancellationToken);

    public async Task<Species?> GetByNameOrScientificNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.Name == name || s.ScientificName == name, cancellationToken);

    public async Task<Species?> GetByName_Authority_GenusAsync(string name, string? authorityName, string? genusName, int? authorityYear, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        // Ana sorguyu oluştur
        var query = _dbSet.AsNoTracking().Where(s => s.Name == name);

        // Authority ve Genus koşulları varsa birleştir
        if (!string.IsNullOrEmpty(authorityName) && !string.IsNullOrEmpty(genusName) && authorityYear.HasValue)
        {
            // Tek sorguda birleştirilmiş hali
            return await query
                .Where(s => s.Authority.Name == authorityName &&
                           s.Authority.Year == authorityYear &&
                           s.Genus.Name == genusName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        // Sadece Authority varsa
        if (!string.IsNullOrEmpty(authorityName) && authorityYear.HasValue)
        {
            return await query
                .Where(s => s.Authority.Name == authorityName &&
                           s.Authority.Year == authorityYear)
                .FirstOrDefaultAsync(cancellationToken);
        }

        // Sadece Genus varsa
        if (!string.IsNullOrEmpty(genusName))
        {
            return await query
                .Where(s => s.Genus.Name == genusName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        // Hiçbir ek koşul yoksa
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<Species?> GetOrCreateSpeciesAsync<TDto>(TDto dto, int? genusId, int? authorityId, CancellationToken cancellationToken) where TDto : ISpeciesImportDto
    {
        var parameters = new[]
        {
            new MySqlParameter("@p0", (object)dto.SpeciesName ?? DBNull.Value),
            new MySqlParameter("@p1", (object)genusId ?? DBNull.Value),
            new MySqlParameter("@p2", (object)authorityId ?? DBNull.Value),
            new MySqlParameter("@p3", (object)dto.ScientificName ?? DBNull.Value),
            new MySqlParameter("@p4", (object)dto.EUName ?? DBNull.Value),
            new MySqlParameter("@p5", (object)dto.FullName ?? DBNull.Value),
            new MySqlParameter("@p6", (object)dto.HesselbarthName ?? DBNull.Value),
            new MySqlParameter("@p7", (object)dto.TurkishName ?? DBNull.Value),
            new MySqlParameter("@p8", (object)dto.EnglishName ?? DBNull.Value),
            new MySqlParameter("@p9", (object)dto.TurkishNamesTrakel ?? DBNull.Value),
            new MySqlParameter("@p10", (object)dto.Trakel ?? DBNull.Value),
            new MySqlParameter("@p11", (object)dto.KocakName ?? DBNull.Value)
        };

        var sql = "CALL sp_GetOrCreateSpecies(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";

        List<Species> result = await dbContext1.Set<Species>()
        .FromSqlRaw(sql, parameters)
        .ToListAsync(cancellationToken);

        Species? species = result.FirstOrDefault();

        return species != null ? species : null;
    }

    public async Task<List<SpeciesStatDto>> GetTopSpeciesAsync(CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Observations).ThenInclude(x=> x.Location)
        .Select(x => new SpeciesStatDto
        {
            SpeciesName = x.Name,
            ObservationCount=x.Observations.Count,
            AverageAltitude = x.Observations.Average(o => o.Location.Altitude1),
        })
        .OrderByDescending(x => x.ObservationCount)
        .Take(5)
        .ToListAsync(cancellationToken);
}

