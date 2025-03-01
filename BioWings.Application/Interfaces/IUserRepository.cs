using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;
public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsExistByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken = default);
}
