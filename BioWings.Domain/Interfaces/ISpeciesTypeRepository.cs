using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface ISpeciesTypeRepository : IGenericRepository<SpeciesType>
{
    Task<SpeciesType?> GetByNameAndDescriptionAsync(string name, string description, CancellationToken cancellationToken = default);
}


