using BioWings.Application.DTOs.UserRoleDtos;
using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;
public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<List<UserRoleGetByUserIdDto>> GetUserRolesByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetUserRoleByUserIdAndRoleIdAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    Task RemoveUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);

}
