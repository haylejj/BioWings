using BioWings.Application.DTOs.UserRoleDtos;
using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Persistence.Repositories;
public class UserRoleRepository(AppDbContext dbContext, ILogger<UserRoleRepository> logger) : GenericRepository<UserRole>(dbContext), IUserRoleRepository
{
    public async Task AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };
        await _dbSet.AddAsync(userRole, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserRole?> GetUserRoleByUserIdAndRoleIdAsync(int userId, int roleId, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    public async Task<List<UserRoleGetByUserIdDto>> GetUserRolesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var roles = await _dbSet.Include(x => x.Role).Where(ur => ur.UserId == userId).Select(x => new UserRoleGetByUserIdDto
        {
            RoleId = x.RoleId,
            RoleName=x.Role.Name
        }).ToListAsync();
        if (roles.Count == 0)
        {
            logger.LogInformation("User with id {UserId} has no roles", userId);
            return new List<UserRoleGetByUserIdDto>();
        }
        logger.LogInformation("User with id {UserId} has roles {Roles}", userId, string.Join(", ", roles.Select(x => x.RoleName)));
        return roles;
    }

    public async Task RemoveUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {

        var userRole = await GetUserRoleByUserIdAndRoleIdAsync(userId, roleId, cancellationToken);
        if (userRole == null)
        {
            logger.LogWarning("User with id {UserId} does not have role with id {RoleId}", userId, roleId);
            return;
        }
        _dbSet.Remove(userRole);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Role with id {RoleId} is removed from user with id {UserId}", roleId, userId);
    }
}
