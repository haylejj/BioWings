using BioWings.Application.Services;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;


public class LoginLogController(ILoginLogService loginLogService) : BaseController
{
    [HttpGet("recent")]
    [AuthorizeDefinition("Giriş Logları", ActionType.Read, "Son giriş denemelerini görüntüler", AreaNames.Admin)]
    public async Task<IActionResult> GetRecentLoginAttempts([FromQuery] int count = 100)
    {
        var result = await loginLogService.GetRecentLoginAttemptsAsync(count);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    [AuthorizeDefinition("Giriş Logları", ActionType.Read, "Belirli bir kullanıcının giriş geçmişini görüntüler", AreaNames.Admin)]
    public async Task<IActionResult> GetUserLoginHistory(int userId)
    {
        var result = await loginLogService.GetUserLoginHistoryAsync(userId);
        return Ok(result);
    }
} 