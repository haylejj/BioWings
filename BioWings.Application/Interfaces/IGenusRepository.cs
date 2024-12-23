using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

public interface IGenusRepository : IGenericRepository<Genus>
{
    Task<Genus?> GetByNameAndFamilyIdAsync(string name, int? familyId, CancellationToken cancellationToken = default);
    Task<Genus?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}


