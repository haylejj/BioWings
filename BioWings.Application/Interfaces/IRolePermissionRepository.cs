using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

/// <summary>
/// RolePermission repository interface
/// </summary>
public interface IRolePermissionRepository
{
    /// <summary>
    /// Tüm RolePermission'ları getirir
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RolePermission listesi</returns>
    Task<List<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// RolePermission'ları toplu olarak ekler
    /// </summary>
    /// <param name="rolePermissions">Eklenecek RolePermission listesi</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// RolePermission'ları toplu olarak siler
    /// </summary>
    /// <param name="rolePermissions">Silinecek RolePermission listesi</param>
    void RemoveRange(IEnumerable<RolePermission> rolePermissions);

    /// <summary>
    /// Belirli bir permission için tüm role permission'ları getirir
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RolePermission listesi</returns>
    Task<List<RolePermission>> GetByPermissionIdAsync(int permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir role için tüm role permission'ları getirir
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RolePermission listesi</returns>
    Task<List<RolePermission>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli bir permission için mevcut tüm role permission'ları siler
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteByPermissionIdAsync(int permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirli role permission'ların var olup olmadığını kontrol eder
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Var mı?</returns>
    Task<bool> ExistsAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tüm permission'lar için role eşleşmelerini getirir (Permission ID'sine göre gruplu)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission ID - Role ID listesi dictionary</returns>
    Task<Dictionary<int, List<int>>> GetAllPermissionRoleMappingsAsync(CancellationToken cancellationToken = default);
}
