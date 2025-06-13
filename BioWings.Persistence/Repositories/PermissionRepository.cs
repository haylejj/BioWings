using BioWings.Application.Interfaces;
using BioWings.Domain.Entities;
using BioWings.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Repositories
{
    /// <summary>
    /// Permission repository implementasyonu
    /// </summary>
    public class PermissionRepository(AppDbContext dbContext) : GenericRepository<Permission>(dbContext), IPermissionRepository
    {
        /// <summary>
        /// PermissionCode'a göre permission bulur
        /// </summary>
        public async Task<Permission?> GetByPermissionCodeAsync(string permissionCode, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.PermissionCode == permissionCode, cancellationToken);
        }

        /// <summary>
        /// PermissionCode'un var olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> ExistsByPermissionCodeAsync(string permissionCode, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(x => x.PermissionCode == permissionCode, cancellationToken);
        }

        /// <summary>
        /// Area'ya göre permission'ları getirir
        /// </summary>
        public async Task<List<Permission>> GetByAreaAsync(string areaName, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(x => x.AreaName == areaName)
                              .OrderBy(x => x.ControllerName)
                              .ThenBy(x => x.ActionName)
                              .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Controller'a göre permission'ları getirir
        /// </summary>
        public async Task<List<Permission>> GetByControllerAsync(string controllerName, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(x => x.ControllerName == controllerName)
                              .OrderBy(x => x.AreaName)
                              .ThenBy(x => x.ActionName)
                              .ToListAsync(cancellationToken);
        }
    }
} 