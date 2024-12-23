using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;
public interface IAuthorityRepository : IGenericRepository<Authority>
{
    Task<Authority?> GetByNameAndYearAsync(string? name, int? year, CancellationToken cancellationToken = default);
}
