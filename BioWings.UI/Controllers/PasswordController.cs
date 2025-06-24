using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.EmailViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BioWings.UI.Controllers;
[AllowAnonymous]
[Route("password")]
public class PasswordController(IHttpClientFactory httpClientFactory, ILogger<PasswordController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    [HttpGet("change")]
    public IActionResult ChangePassword(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Login", "Login");
        }
        ViewBag.Token=token;
        return View(new ChangePasswordViewModel { Token = token, ConfirmPassword="", Password="" });
    }
    [HttpPost("change")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_baseUrl}/Email/changePassword", changePasswordViewModel);
            return response.IsSuccessStatusCode
                ? Json(new { success = true, message = "Şifre değiştirme işlemi başarılı" })
                : Json(new { success = false, message = "Şifre değiştirme işlemi başarısız oldu." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Şifre değiştirme sırasında hata oluştu");
            return Json(new { success = false, message = "Şifre değiştirme işlemi başarısız oldu." });
        }
    }
    [HttpGet("forget")]
    public IActionResult ForgetPassword()
    {
        return View();
    }
    [HttpPost("forget")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Geçersiz e-posta adresi" });
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_baseUrl}/Email/resetPasswordLink",
                new { Email = model.Email });

            return response.IsSuccessStatusCode
                ? Json(new { success = true, message = "Şifre sıfırlama bağlantısı gönderildi" })
                : Json(new { success = false, message = "E-posta gönderimi başarısız oldu" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Şifre sıfırlama isteği sırasında hata oluştu: {Email}", model.Email);
            return Json(new { success = false, message = "Sunucu hatası oluştu" });
        }
    }
}
