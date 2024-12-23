using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

public interface ISubspeciesRepository : IGenericRepository<Subspecies>
{
    Task<IEnumerable<Subspecies>> GetAllWithSpeciesAsync(CancellationToken cancellationToken = default);
    Task<Subspecies?> GetByIdWithSpeciesAsync(int id, CancellationToken cancellationToken = default);
    Task<Subspecies?> GetByNameAndSpeciesIdAsync(string name, int speciesId, CancellationToken cancellationToken = default);
}


