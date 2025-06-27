using BioWings.Domain.Configuration;
using BioWings.UI.ViewModels.UserPasswordViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

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
            // Kullanıcı ID'sini JWT token'dan al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                logger.LogWarning("User ID not found in claims");
                return Json(new { 
                    success = false, 
                    message = "Oturum bilgileri bulunamadı. Lütfen tekrar giriş yapınız." 
                });
            }

            var requestData = new
            {
                UserId = userId,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                ConfirmNewPassword = model.ConfirmNewPassword
            };

            var client = httpClientFactory.CreateClient();
            
            // Add authentication header if available
            var token = HttpContext.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var jsonData = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_baseUrl}/Users/ChangePassword", content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return Json(new { 
                    success = true, 
                    message = "Şifreniz başarıyla değiştirildi. Güvenliğiniz için bir sonraki girişinizde yeni şifrenizi kullanınız." 
                });
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning("Password change failed for user {UserId}. Status: {Status}, Response: {Response}", 
                    userId, response.StatusCode, responseContent);
                
                string errorMessage = GetErrorMessageFromResponse(response.StatusCode, responseContent);
                return Json(new { success = false, message = errorMessage });
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error during password change for user");
            return Json(new { 
                success = false, 
                message = "Bağlantı hatası oluştu. Lütfen internet bağlantınızı kontrol edip tekrar deneyiniz." 
            });
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Timeout during password change for user");
            return Json(new { 
                success = false, 
                message = "İşlem zaman aşımına uğradı. Lütfen tekrar deneyiniz." 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during password change");
            return Json(new { 
                success = false, 
                message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz." 
            });
        }
    }

    private string GetErrorMessageFromResponse(System.Net.HttpStatusCode statusCode, string responseContent)
    {
        try
        {
            // API'den gelen hata mesajını parse etmeye çalış
            using JsonDocument doc = JsonDocument.Parse(responseContent);
            
            // errorList kontrolü
            if (doc.RootElement.TryGetProperty("errorList", out var errorList) && 
                errorList.ValueKind == JsonValueKind.Array && 
                errorList.GetArrayLength() > 0)
            {
                var firstError = errorList[0].GetString();
                return TranslateApiError(firstError);
            }
            
            // message kontrolü
            if (doc.RootElement.TryGetProperty("message", out var message))
            {
                return TranslateApiError(message.GetString());
            }
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to parse error response: {Response}", responseContent);
        }

        // Status code'a göre genel hata mesajları
        return statusCode switch
        {
            System.Net.HttpStatusCode.BadRequest => "Girilen bilgiler geçersiz. Lütfen tüm alanları kontrol ediniz.",
            System.Net.HttpStatusCode.Unauthorized => "Oturum süreniz dolmuş. Lütfen tekrar giriş yapınız.",
            System.Net.HttpStatusCode.Forbidden => "Bu işlem için yetkiniz bulunmuyor.",
            System.Net.HttpStatusCode.NotFound => "Kullanıcı bilgileri bulunamadı.",
            System.Net.HttpStatusCode.Conflict => "Şifre değiştirme işlemi şu anda gerçekleştirilemiyor.",
            System.Net.HttpStatusCode.TooManyRequests => "Çok fazla deneme yapıldı. Lütfen daha sonra tekrar deneyiniz.",
            System.Net.HttpStatusCode.InternalServerError => "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyiniz.",
            System.Net.HttpStatusCode.ServiceUnavailable => "Hizmet şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyiniz.",
            System.Net.HttpStatusCode.RequestTimeout => "İstek zaman aşımına uğradı. Lütfen tekrar deneyiniz.",
            _ => "Şifre değiştirme işlemi başarısız oldu. Lütfen tekrar deneyiniz."
        };
    }

    private string TranslateApiError(string apiError)
    {
        if (string.IsNullOrEmpty(apiError))
            return "Bilinmeyen bir hata oluştu";

        // API'den gelen İngilizce hata mesajlarını Türkçe'ye çevir
        return apiError.ToLower() switch
        {
            var msg when msg.Contains("current password") && msg.Contains("incorrect") => 
                "Mevcut şifreniz yanlış. Lütfen doğru şifrenizi giriniz.",
            var msg when msg.Contains("current password") && msg.Contains("required") => 
                "Mevcut şifre gereklidir.",
            var msg when msg.Contains("new password") && msg.Contains("required") => 
                "Yeni şifre gereklidir.",
            var msg when msg.Contains("password") && msg.Contains("match") => 
                "Şifreler eşleşmiyor. Lütfen tekrar kontrol ediniz.",
            var msg when msg.Contains("password") && msg.Contains("length") => 
                "Şifre en az 8 karakter uzunluğunda olmalıdır.",
            var msg when msg.Contains("password") && msg.Contains("uppercase") => 
                "Şifre en az bir büyük harf içermelidir.",
            var msg when msg.Contains("password") && msg.Contains("lowercase") => 
                "Şifre en az bir küçük harf içermelidir.",
            var msg when msg.Contains("password") && msg.Contains("digit") => 
                "Şifre en az bir rakam içermelidir.",
            var msg when msg.Contains("password") && msg.Contains("special") => 
                "Şifre en az bir özel karakter içermelidir.",
            var msg when msg.Contains("user") && msg.Contains("not found") => 
                "Kullanıcı bulunamadı.",
            var msg when msg.Contains("token") && msg.Contains("invalid") => 
                "Geçersiz oturum. Lütfen tekrar giriş yapınız.",
            var msg when msg.Contains("validation") => 
                "Girilen bilgiler geçerlilik kontrolünden geçemedi.",
            _ => apiError // Çevrilemeyen mesajları olduğu gibi döndür
        };
    }
} 