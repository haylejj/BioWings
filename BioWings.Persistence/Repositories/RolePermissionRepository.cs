using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories;

/// <summary>
/// RolePermission repository implementation
/// </summary>
public class RolePermissionRepository(AppDbContext dbContext) : IRolePermissionRepository
{

    /// <summary>
    /// Tüm RolePermission'ları getirir
    /// </summary>
    public async Task<List<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.RolePermissions.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// RolePermission'ları toplu olarak ekler
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken cancellationToken = default)
    {
        await dbContext.RolePermissions.AddRangeAsync(rolePermissions, cancellationToken);
    }

    /// <summary>
    /// RolePermission'ları toplu olarak siler
    /// </summary>
    public void RemoveRange(IEnumerable<RolePermission> rolePermissions)
    {
        dbContext.RolePermissions.RemoveRange(rolePermissions);
    }

    /// <summary>
    /// Belirli bir permission için tüm role permission'ları getirir
    /// </summary>
    public async Task<List<RolePermission>> GetByPermissionIdAsync(int permissionId, CancellationToken cancellationToken = default)
    {
        return await dbContext.RolePermissions
            .Where(rp => rp.PermissionId == permissionId)
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Belirli bir role için tüm role permission'ları getirir
    /// </summary>
    public async Task<List<RolePermission>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Belirli bir permission için mevcut tüm role permission'ları siler
    /// </summary>
    public async Task DeleteByPermissionIdAsync(int permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await dbContext.RolePermissions
            .Where(rp => rp.PermissionId == permissionId)
            .ToListAsync(cancellationToken);

        dbContext.RolePermissions.RemoveRange(rolePermissions);
    }

    /// <summary>
    /// Belirli role permission'ların var olup olmadığını kontrol eder
    /// </summary>
    public async Task<bool> ExistsAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        return await dbContext.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
    }

    /// <summary>
    /// Tüm permission'lar için role eşleşmelerini getirir (Permission ID'sine göre gruplu)
    /// </summary>
    public async Task<Dictionary<int, List<int>>> GetAllPermissionRoleMappingsAsync(CancellationToken cancellationToken = default)
    {
        var rolePermissions = await dbContext.RolePermissions
            .Select(rp => new { rp.PermissionId, rp.RoleId })
            .ToListAsync(cancellationToken);

        return rolePermissions
            .GroupBy(rp => rp.PermissionId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(rp => rp.RoleId).ToList()
            );
    }
}
