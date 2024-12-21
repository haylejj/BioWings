using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BioWings.Persistence.Repositories;
public class SpeciesRepository(AppDbContext dbContext, ILogger<SpeciesRepository> logger) : GenericRepository<Species>(dbContext), ISpeciesRepository
{
    public IQueryable<Species?> GetUnusedSpeciesRecord() => _dbSet.Include(x => x.Observations).Include(x => x.Subspecies).Where(spec => !spec.Observations.Any() &&  !spec.Subspecies.Any());
    public async Task<Species?> FirstOrDefaultAsync(Expression<Func<Species, bool>> predicate, CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Authority).Include(y => y.Genus).FirstOrDefaultAsync(predicate, cancellationToken);
    public async Task<Species?> GetByHesselbarthNameAsync(string hesselbarthName, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(x => x.HesselbarthName== hesselbarthName, cancellationToken);
    public async Task<Species?> GetByIdWithGenus_Authority_SpeciesTypeAsync(int id, CancellationToken cancellationToken = default) => await _dbSet.Include(s => s.Genus).Include(s => s.Authority).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Species?> GetByKocakNameAsync(string kocakName, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.KocakName == kocakName, cancellationToken);

    public async Task<Species?> GetByScientificNameAsync(string scientificName, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.ScientificName == scientificName, cancellationToken);

    public async Task<Species?> GetByNameOrScientificNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.Name == name || s.ScientificName == name, cancellationToken);

    public async Task<Species?> GetByName_Authority_GenusAsync(string name, string? authorityName, string? genusName, int? authorityYear, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
        .Include(s => s.Authority)
        .Include(s => s.Genus)
        .Where(s => s.Name == name || s.ScientificName == name);

        // Authority bilgilerini kontrol et
        query =!string.IsNullOrEmpty(authorityName) && authorityYear.HasValue
            ? query.Where(s =>
                s.Authority != null &&
                s.Authority.Name == authorityName &&
                s.Authority.Year == authorityYear)
            : query.Where(s => s.Authority == null);

        // Genus bilgilerini kontrol et 
        query =!string.IsNullOrEmpty(genusName)
            ? query.Where(s =>
                s.Genus != null &&
                s.Genus.Name == genusName)
            : query.Where(s => s.Genus == null);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}

