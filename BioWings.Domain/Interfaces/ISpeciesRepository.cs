using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface ISpeciesRepository : IGenericRepository<Species>
{
    Task<Species?> GetByIdWithGenus_Authority_SpeciesTypeAsync(int id, CancellationToken cancellationToken = default);
}


