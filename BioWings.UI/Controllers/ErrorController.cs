using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.UI.Controllers;

/// <summary>
/// Hata sayfaları için MVC controller
/// </summary>
[AllowAnonymous]
public class ErrorController(ILogger<ErrorController> logger) : Controller
{

    /// <summary>
    /// Yetkisiz erişim hata sayfası
    /// </summary>
    /// <returns>Access Denied view</returns>
    public IActionResult AccessDenied()
    {
        logger.LogWarning("Yetkisiz erişim denemesi - User: {UserName}, IP: {IP}, Path: {Path}",
            User?.Identity?.Name ?? "Anonim",
            HttpContext.Connection?.RemoteIpAddress?.ToString(),
            HttpContext.Request.Path);

        ViewBag.Title = "Erişim Engellendi";
        ViewBag.Message = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
        ViewBag.ReturnUrl = Request.Headers.Referer.ToString();

        return View();
    }

    /// <summary>
    /// Sunucu hatası sayfası
    /// </summary>
    /// <returns>Server Error view</returns>
    public IActionResult ServerError()
    {
        logger.LogError("Sunucu hatası sayfası gösteriliyor - Path: {Path}", HttpContext.Request.Path);

        ViewBag.Title = "Sunucu Hatası";
        ViewBag.Message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.";

        return View();
    }

    /// <summary>
    /// Sayfa bulunamadı hatası
    /// </summary>
    /// <returns>Not Found view</returns>
    public IActionResult NotFoundd()
    {
        logger.LogWarning("404 - Sayfa bulunamadı: {Path}", HttpContext.Request.Path);

        ViewBag.Title = "Sayfa Bulunamadı";
        ViewBag.Message = "Aradığınız sayfa bulunamadı veya taşınmış olabilir.";
        ViewBag.RequestedPath = HttpContext.Request.Path;

        return View();
    }

    /// <summary>
    /// Doğrulama hatası sayfası
    /// </summary>
    /// <returns>Validation Error view</returns>
    public IActionResult ValidationError()
    {
        logger.LogWarning("Doğrulama hatası - Path: {Path}", HttpContext.Request.Path);

        ViewBag.Title = "Doğrulama Hatası";
        ViewBag.Message = "Gönderdiğiniz veriler geçersiz. Lütfen bilgilerinizi kontrol edin.";

        return View();
    }

    /// <summary>
    /// Bad Request hatası sayfası
    /// </summary>
    /// <returns>Bad Request view</returns>
    public IActionResult BadRequest()
    {
        logger.LogWarning("Bad Request hatası - Path: {Path}", HttpContext.Request.Path);

        ViewBag.Title = "Geçersiz İstek";
        ViewBag.Message = "Gönderdiğiniz istek geçersiz.";

        return View();
    }

    /// <summary>
    /// Yetkisiz erişim hatası sayfası
    /// </summary>
    /// <returns>Unauthorized view</returns>
    public IActionResult Unauthorized()
    {
        logger.LogWarning("Yetkisiz erişim - Path: {Path}", HttpContext.Request.Path);

        ViewBag.Title = "Yetkisiz Erişim";
        ViewBag.Message = "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.";

        return View();
    }

    /// <summary>
    /// Internal Server Error sayfası
    /// </summary>
    /// <returns>Internal Server Error view</returns>
    public IActionResult InternalServerError()
    {
        logger.LogError("Internal Server Error - Path: {Path}", HttpContext.Request.Path);

        ViewBag.Title = "Sunucu Hatası";
        ViewBag.Message = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.";

        return View();
    }

    /// <summary>
    /// Genel hata sayfası (özel HTTP durum kodları için)
    /// </summary>
    /// <param name="statusCode">HTTP durum kodu</param>
    /// <returns>Error view</returns>
    [Route("Error/{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        logger.LogWarning("HTTP {StatusCode} hatası - Path: {Path}", statusCode, HttpContext.Request.Path);

        ViewBag.StatusCode = statusCode;
        ViewBag.Title = GetErrorTitle(statusCode);
        ViewBag.Message = GetErrorMessage(statusCode);

        return View();
    }

    /// <summary>
    /// HTTP durum koduna göre hata başlığı döndürür
    /// </summary>
    /// <param name="statusCode">HTTP durum kodu</param>
    /// <returns>Hata başlığı</returns>
    private string GetErrorTitle(int statusCode)
    {
        return statusCode switch
        {
            400 => "Geçersiz İstek",
            401 => "Kimlik Doğrulama Gerekli",
            403 => "Erişim Engellendi",
            404 => "Sayfa Bulunamadı",
            500 => "Sunucu Hatası",
            503 => "Hizmet Kullanılamıyor",
            _ => "Bilinmeyen Hata"
        };
    }

    /// <summary>
    /// HTTP durum koduna göre hata mesajı döndürür
    /// </summary>
    /// <param name="statusCode">HTTP durum kodu</param>
    /// <returns>Hata mesajı</returns>
    private string GetErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "Gönderdiğiniz istek geçersiz. Lütfen bilgilerinizi kontrol edin.",
            401 => "Bu sayfaya erişmek için giriş yapmanız gerekiyor.",
            403 => "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.",
            404 => "Aradığınız sayfa bulunamadı veya taşınmış olabilir.",
            500 => "Sunucu tarafında bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
            503 => "Hizmet şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin.",
            _ => "Beklenmeyen bir hata oluştu."
        };
    }
}
