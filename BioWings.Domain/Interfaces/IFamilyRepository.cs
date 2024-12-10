using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;

public interface IFamilyRepository : IGenericRepository<Family>
{
    Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}


