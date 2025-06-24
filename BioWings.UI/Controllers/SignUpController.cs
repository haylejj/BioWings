using BioWings.Application.Features.Results.SignUpResults;
using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.CountryViewModels;
using BioWings.UI.ViewModels.SignUpViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
[AllowAnonymous]
[Route("signup")]
public class SignUpController(IHttpClientFactory httpClientFactory, ILogger<SignUpController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    [HttpGet("")]
    public async Task<IActionResult> SignUp()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/Country");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResponse<List<CountryGetViewModel>>>(content);
            logger.LogInformation("HttpGet request to Country Api was successfully.Countries fetched successfully");
            ViewBag.Countries=data.Data;
        }
        return View();
    }
    [HttpPost("")]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        // Sonuç için hazırlayacağımız JSON nesnesi
        var result = new { success = false, message = "", redirectUrl = "" };

        if (ModelState.IsValid)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_baseUrl}/SignUp", model);
            // API yanıtını okuyalım
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("HttpPost request to SignUp Api was successfully. User signed up successfully");

                return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    ? Json(new
                    {
                        success = true,
                        message = "Kayıt işlemi başarılı. Onay emaili gönderildi.",
                        redirectUrl = "/Login/Login"
                    })
                    : RedirectToAction("Login", "Login");
            }
            else
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ApiResponse<SignUpResult>>(content);
                    var errorMessage = "";

                    if (errorResponse != null && errorResponse.ErrorList != null && errorResponse.ErrorList.Any())
                    {
                        errorMessage = string.Join(" ", errorResponse.ErrorList);

                        foreach (var error in errorResponse.ErrorList)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                        logger.LogWarning($"API returned error: {errorResponse.ErrorList.First()}");
                    }
                    else
                    {
                        errorMessage = "Kayıt işlemi sırasında bir hata oluştu.";
                        ModelState.AddModelError(string.Empty, errorMessage);
                        logger.LogError($"API request failed with status code: {response.StatusCode}, Response: {content}");
                    }

                    // AJAX isteği için JSON yanıtı
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Kayıt işlemi sırasında bir hata oluştu.");
                    logger.LogError($"Failed to parse API response: {ex.Message}, Response content: {content}");

                    // AJAX isteği için JSON yanıtı
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Kayıt işlemi sırasında bir hata oluştu." });
                    }
                }
            }
        }
        else
        {
            // Model doğrulama hataları
            var errorMessage = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            // AJAX isteği için JSON yanıtı
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = errorMessage });
            }
        }

        // Hata durumunda ülke listesini tekrar yükle
        await LoadCountriesAsync();
        return View(model);
    }

    private async Task LoadCountriesAsync()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/Country");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResponse<List<CountryGetViewModel>>>(content);
            ViewBag.Countries = data.Data;
        }
    }
}
