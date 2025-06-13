using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers
{
    /// <summary>
    /// RolePermission yönetimi controller'ı
    /// </summary>
    public class RolePermissionsController(IRolePermissionRepository rolePermissionRepository, IUnitOfWork unitOfWork) : BaseController
    {

        /// <summary>
        /// Tüm permission-role eşleşmelerini getirir
        /// </summary>
        /// <returns>Permission ID - Role ID listesi mapping'i</returns>
        [HttpGet("mappings")]
        public async Task<IActionResult> GetMappings()
        {
            try
            {
                var mappings = await rolePermissionRepository.GetAllPermissionRoleMappingsAsync();
                return Ok(new { IsSuccess = true, Data = mappings });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Permission-Role eşleşmelerini toplu olarak kaydeder
        /// </summary>
        /// <param name="request">Kaydedilecek eşleşmeler</param>
        /// <returns>İşlem sonucu</returns>
        [HttpPost("save")]
        public async Task<IActionResult> SavePermissionRoles([FromBody] SavePermissionRolesRequest request)
        {
            try
            {
                Console.WriteLine($"SavePermissionRoles called with {request?.PermissionRoles?.Count ?? 0} permissions");
                
                if (request?.PermissionRoles == null)
                {
                    Console.WriteLine("Request or PermissionRoles is null");
                    return BadRequest(new { IsSuccess = false, Message = "Geçersiz istek verisi" });
                }

                // Tüm mevcut eşleşmeleri sil
                var existingRolePermissions = await rolePermissionRepository.GetAllAsync();
                Console.WriteLine($"Found {existingRolePermissions.Count} existing role permissions to delete");
                rolePermissionRepository.RemoveRange(existingRolePermissions);

                // Yeni eşleşmeleri ekle
                var newRolePermissions = new List<RolePermission>();
                
                foreach (var permissionRole in request.PermissionRoles)
                {
                    var permissionId = permissionRole.Key;
                    var roleIds = permissionRole.Value ?? new List<int>();

                    Console.WriteLine($"Processing Permission {permissionId} with {roleIds.Count} roles: [{string.Join(",", roleIds)}]");

                    foreach (var roleId in roleIds)
                    {
                        newRolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
                    }
                }

                Console.WriteLine($"Total {newRolePermissions.Count} new role permissions to add");

                if (newRolePermissions.Count != 0)
                {
                    await rolePermissionRepository.AddRangeAsync(newRolePermissions);
                }

                await unitOfWork.SaveChangesAsync();
                Console.WriteLine("Changes saved successfully");

                return Ok(new { IsSuccess = true, Message = $"Permission-Role eşleşmeleri başarıyla kaydedildi. {newRolePermissions.Count} eşleşme eklendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SavePermissionRoles: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Permission-Role kaydetme isteği modeli
    /// </summary>
    public class SavePermissionRolesRequest
    {
        /// <summary>
        /// Permission ID - Role ID listesi mapping'i
        /// </summary>
        public Dictionary<int, List<int>> PermissionRoles { get; set; } = new Dictionary<int, List<int>>();
    }
} 