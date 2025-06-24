using BioWings.Application.Features.Queries.LoginLogQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class LoginLogController(IMediator mediator) : BaseController
{
    [HttpGet("recent")]
    [AuthorizeDefinition("Giriş Logları", ActionType.Read, "Son giriş denemelerini görüntüler", AreaNames.Admin)]
    public async Task<IActionResult> GetRecentLoginAttempts([FromQuery] int count = 100)
    {
        var query = new GetRecentLoginAttemptsQuery { Count = count };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    [HttpGet("user/{userId}")]
    [AuthorizeDefinition("Giriş Logları", ActionType.Read, "Belirli bir kullanıcının giriş geçmişini görüntüler", AreaNames.Admin)]
    public async Task<IActionResult> GetUserLoginHistory(int userId)
    {
        var query = new GetUserLoginHistoryQuery { UserId = userId };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
}