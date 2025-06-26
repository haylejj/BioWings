using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.Areas.Admin.Models.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace BioWings.UI.Areas.Admin.Controllers;

/// <summary>
/// Yetkilendirme tanımlarını yöneten Admin controller'ı
/// </summary>
[Authorize]
[Area("Admin")]
public class AuthorizationController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    /// <summary>
    /// Yetkilendirme tanımları listesi sayfası
    /// </summary>
    /// <returns>Yetkilendirme tanımları ile birlikte Index view'ı</returns>
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"{_baseUrl}/AuthorizeDefinitions");

            if (!response.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Yetkilendirme tanımları yüklenirken bir hata oluştu.";
                return View(new List<AuthorizationDefinitionViewModel>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<AuthorizationDefinitionViewModel>>>(content);

            if (apiResponse?.Data == null)
            {
                ViewData["ErrorMessage"] = "Yetkilendirme tanımları bulunamadı.";
                return View(new List<AuthorizationDefinitionViewModel>());
            }

            return View(apiResponse.Data);
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = $"Beklenmeyen bir hata oluştu: {ex.Message}";
            return View(new List<AuthorizationDefinitionViewModel>());
        }
    }

    /// <summary>
    /// Yetkilendirme tanımlarını veritabanıyla senkronize eder
    /// </summary>
    /// <returns>Index sayfasına yönlendirme</returns>
    [HttpPost]
    public async Task<IActionResult> Sync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsync($"{_baseUrl}/Permissions/sync", new StringContent("", Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<dynamic>>(content);

                if (apiResponse?.IsSuccess == true)
                {
                    var addedCount = (int)(apiResponse.Data.addedCount ?? 0);
                    var removedCount = (int)(apiResponse.Data.removedCount ?? 0);
                    var totalPermissions = (int)(apiResponse.Data.totalPermissions ?? 0);

                    var message = $"Senkronizasyon tamamlandı. ";
                    if (addedCount > 0)
                        message += $"{addedCount} yeni yetki eklendi. ";
                    if (removedCount > 0)
                        message += $"{removedCount} eski yetki silindi. ";
                    message += $"Toplam: {totalPermissions} yetki.";

                    TempData["SuccessMessage"] = message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Senkronizasyon sırasında bir hata oluştu.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Senkronizasyon işlemi başarısız oldu.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Beklenmeyen bir hata oluştu: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Permission-Role eşleşmeleri yönetim sayfası
    /// </summary>
    /// <returns>Permission-Role yönetim view'ı</returns>
    public async Task<IActionResult> PermissionRoles()
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");

            // Permission'ları al
            var permissionsResponse = await client.GetAsync($"{_baseUrl}/Permissions");
            if (!permissionsResponse.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Permission verileri yüklenirken bir hata oluştu.";
                return View(new PermissionRoleManagementViewModel());
            }
            var client2 = httpClientFactory.CreateClient("ApiClient");
            // Rolleri al
            var rolesResponse = await client2.GetAsync($"{_baseUrl}/Roles");
            if (!rolesResponse.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Role verileri yüklenirken bir hata oluştu.";
                return View(new PermissionRoleManagementViewModel());
            }
            var client3 = httpClientFactory.CreateClient("ApiClient");
            // Permission-Role eşleşmelerini al
            var mappingsResponse = await client3.GetAsync($"{_baseUrl}/RolePermissions/mappings");

            var permissionsContent = await permissionsResponse.Content.ReadAsStringAsync();
            var rolesContent = await rolesResponse.Content.ReadAsStringAsync();

            var permissionsApiResponse = JsonConvert.DeserializeObject<ApiResponse<List<dynamic>>>(permissionsContent);
            var rolesApiResponse = JsonConvert.DeserializeObject<ApiResponse<List<dynamic>>>(rolesContent);

            var viewModel = new PermissionRoleManagementViewModel();

            // Rolleri map et
            if (rolesApiResponse?.Data != null)
            {
                viewModel.Roles = rolesApiResponse.Data.Select(r => new RoleInfo
                {
                    Id = (int)r.id,
                    Name = (string)r.name
                }).ToList();
            }

            // Permission'ları map et
            if (permissionsApiResponse?.Data != null)
            {
                viewModel.Permissions = permissionsApiResponse.Data.Select(p => new PermissionWithRolesViewModel
                {
                    PermissionId = (int)p.id,
                    PermissionCode = (string)p.permissionCode,
                    ControllerName = (string)p.controllerName,
                    ActionName = (string)p.actionName,
                    Definition = (string)p.definition,
                    ActionType = (string)p.actionType,
                    HttpType = (string)p.httpType,
                    MenuName = (string)p.menuName,
                    AreaName = (string)p.areaName,
                    SelectedRoleIds = new List<int>()
                }).OrderBy(x => x.ControllerName).ThenBy(x => x.ActionName).ToList();
            }

            // Eşleşmeleri map et
            if (mappingsResponse.IsSuccessStatusCode)
            {
                var mappingsContent = await mappingsResponse.Content.ReadAsStringAsync();
                var mappingsApiResponse = JsonConvert.DeserializeObject<ApiResponse<Dictionary<int, List<int>>>>(mappingsContent);

                if (mappingsApiResponse?.Data != null)
                {
                    foreach (var permission in viewModel.Permissions)
                    {
                        if (mappingsApiResponse.Data.ContainsKey(permission.PermissionId))
                        {
                            permission.SelectedRoleIds = mappingsApiResponse.Data[permission.PermissionId];
                        }
                    }
                }
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = $"Beklenmeyen bir hata oluştu: {ex.Message}";
            return View(new PermissionRoleManagementViewModel());
        }
    }

    /// <summary>
    /// Permission-Role eşleşmelerini kaydeder
    /// </summary>
    /// <param name="form">Form verileri</param>
    /// <returns>PermissionRoles sayfasına yönlendirme</returns>
    ///
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> SavePermissionRoles(IFormCollection form)
    {
        try
        {
            var permissionRoles = new Dictionary<int, List<int>>();

            // Form'dan permission-role eşleşmelerini parse et
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("permissionRoles[") && key.EndsWith("]"))
                {
                    // "permissionRoles[123]" formatından permission ID'yi çıkar
                    var permissionIdStr = key.Substring("permissionRoles[".Length, key.Length - "permissionRoles[".Length - 1);
                    if (int.TryParse(permissionIdStr, out int permissionId))
                    {
                        var roleIds = form[key].Select(v => int.TryParse(v, out int roleId) ? roleId : 0)
                                              .Where(id => id > 0)
                                              .ToList();

                        if (roleIds.Any())
                        {
                            permissionRoles[permissionId] = roleIds;
                        }
                    }
                }
            }

            var client = httpClientFactory.CreateClient("ApiClient");
            var request = new SavePermissionRolesRequest { PermissionRoles = permissionRoles };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_baseUrl}/RolePermissions/save", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Permission-Role eşleşmeleri başarıyla kaydedildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kaydetme işlemi sırasında bir hata oluştu.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Beklenmeyen bir hata oluştu: {ex.Message}";
        }

        return RedirectToAction(nameof(PermissionRoles));
    }
}
