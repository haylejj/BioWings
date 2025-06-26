using BioWings.Application.Features.Commands.LogoutCommands;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using BioWings.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BioWings.WebAPI.Controllers;

public class LogoutController(IMediator mediator, IIpAddressService ipAddressService) : BaseController
{
    [HttpPost]
    [AuthorizeDefinition("Kullanıcı İşlemleri", ActionType.Write, "Kullanıcı çıkış işlemi", AreaNames.Public)]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        // IP adresi ve User Agent bilgisini ekle
        command.IpAddress = ipAddressService.GetClientIpAddress(HttpContext);
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
        
        // Kullanıcı bilgilerini al
        if (User.Identity.IsAuthenticated)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                command.UserId = userId;
            }
            command.UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        }

        var result = await mediator.Send(command);
        return CreateResult(result);
    }
} 