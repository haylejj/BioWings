using BioWings.Application.Features.Commands.EmailCommands;
using BioWings.Application.Features.Commands.EncryptionCommands;
using BioWings.Application.Features.Results.EncryptionResults;
using BioWings.Application.Results;
using BioWings.UI.ViewModels.EmailViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;
[AllowAnonymous]
public class EmailController(IHttpClientFactory httpClientFactory, ILogger<EmailController> logger) : Controller
{
    public async Task<IActionResult> EmailConfirm([FromQuery] string code)
    {
        // Başarısız durumları için kullanacağımız genel model
        var model = new EmailConfirmViewModel
        {
            IsSuccess = false,
            Message = "Doğrulama işlemi başarısız oldu.",
            Email = string.Empty
        };

        if (string.IsNullOrEmpty(code))
        {
            model.Message = "Geçersiz doğrulama kodu. Lütfen e-postanızdaki bağlantıyı kontrol edin.";
            return View("EmailConfirm", model);
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            var responseDecrypt = await client.PostAsJsonAsync("https://localhost:7128/api/Encryption/decrypt", new DecryptCommand(code));
            if (!responseDecrypt.IsSuccessStatusCode)
            {
                model.Message = "Geçersiz doğrulama kodu formatı.";
                return View("EmailConfirm", model);
            }
            var decrypted = await responseDecrypt.Content.ReadFromJsonAsync<ApiResponse<DecryptResult>>();
            var email = decrypted.Data.Email;
            var token = decrypted.Data.Token;
            model.Email = email; // E-posta adresini modele ekle (yeniden gönderme için kullanılabilir)

            // Email doğrulama işlemini gerçekleştir
            var client2 = httpClientFactory.CreateClient();
            var response = await client2.PostAsJsonAsync("https://localhost:7128/api/Email/confirm", new EmailConfirmCommand { Email = email, Token = token });

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email confirmed successfully for: {Email}", email);

                // Başarılı durumu
                model.IsSuccess = true;
                model.Message = "E-posta adresiniz başarıyla doğrulandı. Artık BioWings sistemine giriş yapabilirsiniz.";
                return View("EmailConfirm", model);
            }
            else
            {
                // API yanıtını oku
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    model.Message = "E-posta doğrulama işlemi başarısız oldu. Doğrulama bağlantınızın süresi dolmuş olabilir.";
                    model.ShowResendButton = true;
                }
                else
                {
                    model.Message = "E-posta doğrulama işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                }

                return View("EmailConfirm", model);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during email confirmation: {Message}", ex.Message);
            model.Message = "Doğrulama işleminde bir hata oluştu. Lütfen tekrar deneyin.";
            return View("EmailConfirm", model);
        }
    }
}
