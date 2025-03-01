using BioWings.Application.Features.Results.SignUpResults;
using BioWings.Application.Results;
using BioWings.UI.ViewModels.CountryViewModels;
using BioWings.UI.ViewModels.SignUpViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
public class SignUpController(IHttpClientFactory httpClientFactory, ILogger<SignUpController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> SignUp()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7128/api/Country");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResponse<List<CountryGetViewModel>>>(content);
            logger.LogInformation("HttpGet request to Country Api was successfully.Countries fetched successfully");
            ViewBag.Countries=data.Data;
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        // Sonuç için hazırlayacağımız JSON nesnesi
        var result = new { success = false, message = "", redirectUrl = "" };

        if (ModelState.IsValid)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7128/api/SignUp", model);
            // API yanıtını okuyalım
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("HttpPost request to SignUp Api was successfully. User signed up successfully");

                // AJAX isteği için JSON yanıtı
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = true,
                        message = "Kayıt işlemi başarılı. Onay emaili gönderildi.",
                        redirectUrl = "/Login/Login"
                    });
                }

                // Normal form submit için redirect
                return RedirectToAction("Login", "Login");
            }
            else
            {
                try
                {
                    // API'dan dönen ServiceResult yanıtını dönüştür
                    var errorResponse = JsonConvert.DeserializeObject<ApiResponse<SignUpResult>>(content);
                    var errorMessage = "";

                    if (errorResponse != null && errorResponse.ErrorList != null && errorResponse.ErrorList.Any())
                    {
                        // API'dan gelen özel hata mesajlarını topla
                        errorMessage = string.Join(" ", errorResponse.ErrorList);

                        // ModelState'e ekle
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
        var response = await client.GetAsync("https://localhost:7128/api/Country");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResponse<List<CountryGetViewModel>>>(content);
            ViewBag.Countries = data.Data;
        }
    }
}
