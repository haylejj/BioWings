using BioWings.Application.Interfaces;
using BioWings.Domain.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Security.Claims;

namespace BioWings.WebAPI.Filters;

/// <summary>
/// Yetkilendirme kontrolü yapan filter - Her istekte veritabanından kontrol yapar
/// </summary>
public class PermissionAuthorizationFilter(
    IUserRoleRepository userRoleRepository,
    IRolePermissionRepository rolePermissionRepository,
    IPermissionRepository permissionRepository,
    ILogger<PermissionAuthorizationFilter> logger) : IAsyncAuthorizationFilter
{

    /// <summary>
    /// Yetkilendirme kontrolü gerçekleştirir
    /// </summary>
    /// <param name="context">Authorization context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            // 1. AuthorizeDefinition attribute'unu bul
            var authorizeDefinition = GetAuthorizeDefinitionAttribute(context);

            // Eğer attribute yoksa, yetkilendirme kontrolü yapma
            if (authorizeDefinition == null)
            {
                logger.LogDebug("AuthorizeDefinition attribute bulunamadı. Yetkilendirme kontrolü atlanıyor.");
                return;
            }

            // 2. Kullanıcı giriş yapmış mı kontrol et
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                logger.LogWarning("Kullanıcı giriş yapmamış. AccessDenied'a yönlendiriliyor.");
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                return;
            }

            // 3. Kullanıcı ID'sini al
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                logger.LogWarning("Kullanıcı ID'si alınamadı. AccessDenied'a yönlendiriliyor.");
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                return;
            }

            // 4. PermissionCode oluştur
            var permissionCode = GeneratePermissionCode(context, authorizeDefinition);
            logger.LogDebug("Oluşturulan PermissionCode: {PermissionCode}", permissionCode);

            // 5. Kullanıcının bu izne sahip olup olmadığını kontrol et
            var hasPermission = await HasUserPermissionAsync(userId, permissionCode);

            if (!hasPermission)
            {
                logger.LogWarning("Kullanıcı {UserId} için {PermissionCode} izni bulunamadı.", userId, permissionCode);

                // API request ise JSON response döndür
                if (IsApiRequest(context.HttpContext))
                {
                    context.Result = new JsonResult(new
                    {
                        error = "Access Denied",
                        message = "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.",
                        timestamp = DateTime.UtcNow
                    })
                    {
                        StatusCode = 403
                    };
                }
                else
                {
                    //UI request ise AccessDenied sayfasına yönlendir
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                }
                return;
            }

            logger.LogDebug("Kullanıcı {UserId} için {PermissionCode} izni onaylandı.", userId, permissionCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Yetkilendirme kontrolü sırasında hata oluştu.");
            context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
        }
    }

    /// <summary>
    /// Action veya Controller'dan AuthorizeDefinition attribute'unu bulur
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <returns>AuthorizeDefinition attribute veya null</returns>
    private AuthorizeDefinitionAttribute? GetAuthorizeDefinitionAttribute(AuthorizationFilterContext context)
    {
        // Önce action'dan attribute'u aramaya çalış
        var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (actionDescriptor == null) return null;

        // Action method'undan attribute'u al
        var actionAttribute = actionDescriptor.MethodInfo
            .GetCustomAttribute<AuthorizeDefinitionAttribute>();

        if (actionAttribute != null)
            return actionAttribute;

        // Action'da yoksa controller'dan al
        var controllerAttribute = actionDescriptor.ControllerTypeInfo
            .GetCustomAttribute<AuthorizeDefinitionAttribute>();

        return controllerAttribute;
    }

    /// <summary>
    /// AuthorizeDefinition attribute'undan PermissionCode oluşturur
    /// Format: {AreaName}.{ControllerName}.{ActionName}.{ActionType}.{HttpMethod}
    /// </summary>
    /// <param name="context">Authorization context</param>
    /// <param name="authorizeDefinition">AuthorizeDefinition attribute</param>
    /// <returns>Permission code</returns>
    private string GeneratePermissionCode(AuthorizationFilterContext context, AuthorizeDefinitionAttribute authorizeDefinition)
    {
        var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;

        var areaName = string.IsNullOrWhiteSpace(authorizeDefinition.AreaName) ? "Global" : authorizeDefinition.AreaName;
        var controllerName = actionDescriptor?.ControllerName ?? "Unknown";
        var actionName = actionDescriptor?.ActionName ?? "Unknown";
        var actionType = authorizeDefinition.ActionType.ToString();
        var httpMethod = context.HttpContext.Request.Method;

        var permissionCode = $"{areaName}.{controllerName}.{actionName}.{actionType}.{httpMethod}";

        return permissionCode;
    }

    /// <summary>
    /// Kullanıcının belirli bir izne sahip olup olmadığını kontrol eder
    /// </summary>
    /// <param name="userId">Kullanıcı ID</param>
    /// <param name="permissionCode">İzin kodu</param>
    /// <returns>İzne sahip mi?</returns>
    private async Task<bool> HasUserPermissionAsync(int userId, string permissionCode)
    {
        try
        {
            // 1. Kullanıcının rollerini al
            var userRoles = await userRoleRepository.GetUserRolesByUserIdAsync(userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any())
            {
                logger.LogDebug("Kullanıcı {UserId} için hiç rol bulunamadı.", userId);
                return false;
            }

            // 2. Tüm rollerin izinlerini al
            var allPermissions = new List<int>();
            foreach (var roleId in roleIds)
            {
                var rolePermissions = await rolePermissionRepository.GetByRoleIdAsync(roleId);
                allPermissions.AddRange(rolePermissions.Select(rp => rp.PermissionId));
            }

            if (!allPermissions.Any())
            {
                logger.LogDebug("Kullanıcı {UserId} rolleri için hiç izin bulunamadı.", userId);
                return false;
            }

            // 3. İzin kodunu kontrol et
            foreach (var permissionId in allPermissions.Distinct())
            {
                var permission = await permissionRepository.GetByIdAsync(permissionId);
                if (permission != null && permission.PermissionCode.Equals(permissionCode, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogDebug("Kullanıcı {UserId} için {PermissionCode} izni bulundu.", userId, permissionCode);
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı {UserId} için {PermissionCode} izin kontrolü sırasında hata oluştu.", userId, permissionCode);
            return false;
        }
    }

    /// <summary>
    /// Request'in API isteği olup olmadığını kontrol eder
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <returns>API request mi?</returns>
    private bool IsApiRequest(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value?.ToLower() ?? "";
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();

        // API path kontrolü
        if (path.StartsWith("/api/"))
            return true;

        // Accept header kontrolü (JSON isteyen client'lar)
        if (acceptHeader.Contains("application/json") && !acceptHeader.Contains("text/html"))
            return true;

        // Content-Type kontrolü (AJAX istekleri)
        var contentType = httpContext.Request.ContentType?.ToLower() ?? "";
        if (contentType.Contains("application/json"))
            return true;

        // X-Requested-With header kontrolü (AJAX)
        return httpContext.Request.Headers.ContainsKey("X-Requested-With");
    }
}
