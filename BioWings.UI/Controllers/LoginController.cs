using BioWings.Application.DTOs.LoginDtos;
using BioWings.Application.Features.Results.LoginResults;
using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BioWings.UI.Controllers;
[AllowAnonymous]
public class LoginController(IHttpClientFactory httpClientFactory, ILogger<LoginController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                logger.LogWarning("Validation failed for {Email}: {Errors}", loginDto.Email, string.Join(", ", errors));
                return View(loginDto);
            }

            // API isteği gönderme
            var client = httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


            var response = await client.PostAsync($"{_baseUrl}/Login", content);


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
                    TempData["LoginError"] = result.ErrorList?.FirstOrDefault() ?? "Login failed.";
                }
            }
            else
            {
                TempData["LoginError"] = "Unable to login.";
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP Request Exception for {Email}", loginDto.Email);
            TempData["LoginError"] = "Connection error. Please try again.";
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON Exception for {Email}", loginDto.Email);
            TempData["LoginError"] = "Server response error.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login error for {Email}", loginDto.Email);
            TempData["LoginError"] = "An unexpected error occurred.";
        }

        return View();
    }
}
