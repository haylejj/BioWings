using BioWings.Application.DTOs.LoginDtos;
using BioWings.Application.Features.Results.LoginResults;
using BioWings.Application.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BioWings.UI.Controllers;
public class LoginController(IHttpClientFactory httpClientFactory, ILogger<LoginController> logger) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // ModelState hatalarını loglayalım
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
                return View(loginDto);
            }

            // API isteği gönderme
            var client = httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


            var response = await client.PostAsync("https://localhost:7128/api/Login", content);


            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponse<LoginResult>>(apiResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result.IsSuccess && result.Data != null)
                {
                    var claims = new List<Claim>
                {
                    new(ClaimTypes.Email, loginDto.Email),
                    new("AccessToken", result.Data.Token),
                    new("RefreshToken", result.Data.RefreshToken),
                    new("TokenExpiration", result.Data.Expiration.ToString("o"))
                };

                    if (result.Data.Roles != null)
                    {
                        foreach (var role in result.Data.Roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Cookie ayarları
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = loginDto.RememberMe,
                        ExpiresUtc = result.Data.Expiration,
                        AllowRefresh = true
                    };

                    // Cookie oluştur
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        authProperties);

                    return RedirectToAction("Index", "Observation");
                }
                else
                {
                    var errorMessage = result.ErrorList?.FirstOrDefault() ?? "Giriş başarısız oldu.";
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Giriş yapılamadı. HTTP Status: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine();
            logger.LogError(ex, "HTTP Request Exception");
            ModelState.AddModelError(string.Empty, $"API bağlantı hatası: {ex.Message}");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON Exception");
            ModelState.AddModelError(string.Empty, "Sunucu yanıtı işlenirken hata oluştu.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Exception");
            ModelState.AddModelError(string.Empty, "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
        }

        return View();
    }
}
