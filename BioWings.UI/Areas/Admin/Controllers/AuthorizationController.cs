using BioWings.Application.Results;
using BioWings.UI.Areas.Admin.Models.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BioWings.UI.Areas.Admin.Controllers
{
    /// <summary>
    /// Yetkilendirme tanımlarını yöneten Admin controller'ı
    /// </summary>
    [Area("Admin")]
    public class AuthorizationController(IHttpClientFactory httpClientFactory) : Controller
    {
        /// <summary>
        /// Yetkilendirme tanımları listesi sayfası
        /// </summary>
        /// <returns>Yetkilendirme tanımları ile birlikte Index view'ı</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:7128/api/AuthorizeDefinitions");
                
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
                var client = httpClientFactory.CreateClient();
                var response = await client.PostAsync("https://localhost:7128/api/Permissions/sync", new StringContent("", Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int>>(content);
                    
                    if (apiResponse?.IsSuccess == true)
                    {
                        TempData["SuccessMessage"] = $"Senkronizasyon tamamlandı. {apiResponse.Data} yeni yetki eklendi.";
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
    }
} 