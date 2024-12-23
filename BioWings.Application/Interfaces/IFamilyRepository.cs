using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

public interface IFamilyRepository : IGenericRepository<Family>
{
    Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}


