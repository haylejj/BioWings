using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.Areas.Admin.Models.User;
using BioWings.UI.ViewModels.CountryViewModels;
using BioWings.UI.ViewModels.UserPasswordViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace BioWings.UI.Areas.Admin.Controllers;
[Authorize]
[Area("Admin")]
public class UserController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options, ILogger<UserController> logger) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;

    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"{_baseUrl}/Users");
        if (!response.IsSuccessStatusCode)
        {
            ViewData["ErrorMessage"]="An error occurred while fetching the data";
            return View();
        }
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse<List<UserGetViewModel>>>(content);
        //dropdown list for countries
        var countryClient = httpClientFactory.CreateClient("ApiClient");
        var countryResponse = await countryClient.GetAsync($"{_baseUrl}/Country");
        if (!countryResponse.IsSuccessStatusCode)
        {
            ViewData["ErrorMessage"]="An error occurred while fetching the countries";
            return View();
        }
        var countryContent = await countryResponse.Content.ReadAsStringAsync();
        var countries = JsonConvert.DeserializeObject<ApiResponse<List<CountryGetViewModel>>>(countryContent);
        ViewBag.Countries = countries.Data;


        if (!data.Data.Any())
        {
            return View(new UserViewModel { GetViewModels = new List<UserGetViewModel>() });
        }
        var model = new UserViewModel
        {
            GetViewModels = data.Data
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UserViewModel model)
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var content = new StringContent(JsonConvert.SerializeObject(model.UpdateViewModel), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{_baseUrl}/Users", content);
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessageForUpdate"]="An error occurred while updating the user";
            return RedirectToAction("Index");
        }
        TempData["SuccessMessageForUpdate"] = "Kullanıcı başarıyla güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => x.Value.Errors.First().ErrorMessage)
                .ToList();

            string errorMessage = errors.Count > 0
                ? errors.First()
                : "Lütfen tüm alanları doğru şekilde doldurunuz";

            return Json(new { success = false, message = errorMessage });
        }

        try
        {
            // Kullanıcı email'ini cookie'den al
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
            {
                logger.LogWarning("User email not found in claims");
                return Json(new
                {
                    success = false,
                    message = "Oturum bilgileri bulunamadı. Lütfen tekrar giriş yapınız."
                });
            }

            var command = new ChangePasswordCommand
            {
                UserEmail = emailClaim.Value,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                ConfirmNewPassword = model.ConfirmNewPassword
            };

            var client = httpClientFactory.CreateClient("ApiClient");

            var jsonData = System.Text.Json.JsonSerializer.Serialize(command);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_baseUrl}/Users/ChangePassword", content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Password changed successfully for admin user {Email}", emailClaim.Value);
                return Json(new
                {
                    success = true,
                    message = "Şifreniz başarıyla değiştirildi. Güvenliğiniz için bir sonraki girişinizde yeni şifrenizi kullanınız."
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning("Password change failed for admin user {Email}. Status: {Status}, Response: {Response}",
                    emailClaim.Value, response.StatusCode, responseContent);

                return Json(new { success = false, message = "Şifre değiştirme işlemi başarısız oldu. Lütfen bilgilerinizi kontrol edip tekrar deneyiniz." });
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error during password change for admin user");
            return Json(new
            {
                success = false,
                message = "Bağlantı hatası oluştu. Lütfen internet bağlantınızı kontrol edip tekrar deneyiniz."
            });
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Timeout during password change for admin user");
            return Json(new
            {
                success = false,
                message = "İşlem zaman aşımına uğradı. Lütfen tekrar deneyiniz."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during password change for admin user");
            return Json(new
            {
                success = false,
                message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz."
            });
        }
    }


}
