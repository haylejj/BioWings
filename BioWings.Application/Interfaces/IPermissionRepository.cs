using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

/// <summary>
/// Permission repository interface
/// </summary>
public interface IPermissionRepository : IGenericRepository<Permission>
{
    /// <summary>
    /// PermissionCode'a göre permission bulur
    /// </summary>
    /// <param name="permissionCode">Permission kodu</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission entity</returns>
    Task<Permission?> GetByPermissionCodeAsync(string permissionCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// PermissionCode'un var olup olmadığını kontrol eder
    /// </summary>
    /// <param name="permissionCode">Permission kodu</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Var mı?</returns>
    Task<bool> ExistsByPermissionCodeAsync(string permissionCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Area'ya göre permission'ları getirir
    /// </summary>
    /// <param name="areaName">Area adı</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission listesi</returns>
    Task<List<Permission>> GetByAreaAsync(string areaName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Controller'a göre permission'ları getirir
    /// </summary>
    /// <param name="controllerName">Controller adı</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Permission listesi</returns>
    Task<List<Permission>> GetByControllerAsync(string controllerName, CancellationToken cancellationToken = default);
}
