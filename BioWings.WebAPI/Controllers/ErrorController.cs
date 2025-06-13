using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers
{
    /// <summary>
    /// Hata sayfaları için controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorController(ILogger<ErrorController> logger) : ControllerBase
    {

        /// <summary>
        /// Yetkisiz erişim hatası
        /// </summary>
        /// <returns>403 Forbidden response</returns>
        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            logger.LogWarning("Yetkisiz erişim denemesi: {UserName} - {RequestPath}", 
                User?.Identity?.Name ?? "Anonymous", 
                HttpContext.Request.Path);

            return StatusCode(403, new
            {
                error = "Access Denied",
                message = "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.",
                timestamp = DateTime.UtcNow,
                path = HttpContext.Request.Path.ToString()
            });
        }

        /// <summary>
        /// Genel hata sayfası
        /// </summary>
        /// <returns>500 Internal Server Error response</returns>
        [HttpGet("server-error")]
        public IActionResult ServerError()
        {
            return StatusCode(500, new
            {
                error = "Internal Server Error",
                message = "Sunucu tarafında bir hata oluştu.",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Bulunamayan sayfa hatası
        /// </summary>
        /// <returns>404 Not Found response</returns>
        [HttpGet("not-found")]
        public IActionResult NotFoundd()
        {
            return StatusCode(404, new
            {
                error = "Not Found",
                message = "Aradığınız sayfa bulunamadı.",
                timestamp = DateTime.UtcNow,
                path = HttpContext.Request.Path.ToString()
            });
        }
    }
} 