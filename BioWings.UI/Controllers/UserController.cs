using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.UserPasswordViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BioWings.UI.Controllers;

[Authorize]
[Route("User")]
public class UserController(IHttpClientFactory httpClientFactory, ILogger<UserController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;

    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost("ChangePassword")]
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

            var jsonData = JsonSerializer.Serialize(command);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_baseUrl}/Users/ChangePassword", content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Password changed successfully for user {Email}", emailClaim.Value);
                
                // Şifre değiştirme başarılı olduğunda server-side logout işlemi yap
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                return Json(new
                {
                    success = true,
                    message = "Şifreniz başarıyla değiştirildi. Güvenliğiniz için tekrar giriş yapmanız gerekmektedir.",
                    shouldLogout = true // JavaScript tarafında kullanılacak flag
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning("Password change failed for user {Email}. Status: {Status}, Response: {Response}",
                    emailClaim.Value, response.StatusCode, responseContent);

                return Json(new { success = false, message = "Şifre değiştirme işlemi başarısız oldu. Lütfen bilgilerinizi kontrol edip tekrar deneyiniz." });
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error during password change for user");
            return Json(new
            {
                success = false,
                message = "Bağlantı hatası oluştu. Lütfen internet bağlantınızı kontrol edip tekrar deneyiniz."
            });
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Timeout during password change for user");
            return Json(new
            {
                success = false,
                message = "İşlem zaman aşımına uğradı. Lütfen tekrar deneyiniz."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during password change");
            return Json(new
            {
                success = false,
                message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz."
            });
        }
    }

}