using BioWings.Domain.Entities;

namespace BioWings.Domain.Interfaces;
public interface IAuthorityRepository : IGenericRepository<Authority>
{
    Task<Authority?> GetByNameAndYearAsync(string? name, int? year, CancellationToken cancellationToken = default);
}
