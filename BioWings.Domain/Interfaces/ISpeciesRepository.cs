using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface ISpeciesRepository : IGenericRepository<Species>
{
    Task<Species?> GetByIdWithGenus_Authority_SpeciesTypeAsync(int id, CancellationToken cancellationToken = default);
    Task<Species?> GetByScientificNameAsync(string scientificName, CancellationToken cancellationToken = default);
    Task<Species?> GetByKocakNameAsync(string kocakName, CancellationToken cancellationToken = default);
    Task<Species?> GetByHesselbarthNameAsync(string hesselbarthName, CancellationToken cancellationToken = default);
}


