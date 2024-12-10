using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface ISpeciesTypeRepository : IGenericRepository<SpeciesType>
{
    Task<SpeciesType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}


