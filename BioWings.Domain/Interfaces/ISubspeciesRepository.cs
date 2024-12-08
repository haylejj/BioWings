using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface ISubspeciesRepository : IGenericRepository<Subspecies>
{
    Task<IEnumerable<Subspecies>> GetAllWithSpeciesAsync(CancellationToken cancellationToken = default);
    Task<Subspecies?> GetByIdWithSpeciesAsync(int id, CancellationToken cancellationToken = default);
}


