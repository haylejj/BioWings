using BioWings.Application.Features.Commands.LogoutCommands;
using BioWings.Domain.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace BioWings.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class AdminLogoutController(IHttpClientFactory httpClientFactory, ILogger<AdminLogoutController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // API'ye logout isteği gönder - ApiClient otomatik olarak Bearer token ekler
            var client = httpClientFactory.CreateClient("ApiClient");
            var logoutCommand = new LogoutCommand
            {
                UserEmail = User.Identity.Name
            };
            var jsonData = JsonSerializer.Serialize(logoutCommand);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_baseUrl}/Logout", content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Admin user successfully logged out via API");
            }
            else
            {
                logger.LogWarning("API logout failed for admin user, but continuing with local logout");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling logout API for admin user, but continuing with local logout");
        }

        // Yerel cookie'leri temizle
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Login sayfasına yönlendir
        return RedirectToAction("Login", "AdminLogin", new { area = "Admin" });
    }


} 