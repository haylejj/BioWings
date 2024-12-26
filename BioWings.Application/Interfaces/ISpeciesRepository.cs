using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Application.Interfaces;

public interface ISpeciesRepository : IGenericRepository<Species>
{
    Task<Species?> GetByIdWithGenus_Authority_SpeciesTypeAsync(int id, CancellationToken cancellationToken = default);
    Task<Species?> GetByScientificNameAsync(string scientificName, CancellationToken cancellationToken = default);
    Task<Species?> GetByKocakNameAsync(string kocakName, CancellationToken cancellationToken = default);
    Task<Species?> GetByHesselbarthNameAsync(string hesselbarthName, CancellationToken cancellationToken = default);
    Task<Species?> FirstOrDefaultAsync(Expression<Func<Species, bool>> predicate, CancellationToken cancellationToken = default);
    IQueryable<Species?> GetUnusedSpeciesRecord();
    Task<Species?> GetByNameOrScientificNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Species?> GetByName_Authority_GenusAsync(string name, string? authorityName, string? genusName, int? authorityYear, CancellationToken cancellationToken = default);
    Task<Species?> GetOrCreateSpeciesAsync<TDto>(TDto dto, int? genusId, int? authorityId, CancellationToken cancellationToken) where TDto : ISpeciesImportDto;
}


