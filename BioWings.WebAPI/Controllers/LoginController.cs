using BioWings.Application.Features.Commands.LoginCommands;
using BioWings.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;


public class LoginController(IMediator mediator, IIpAddressService ipAddressService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        // IP adresi ve User Agent bilgisini ekle
        command.IpAddress = ipAddressService.GetClientIpAddress(HttpContext);
        command.UserAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

        var result = await mediator.Send(command);
        return CreateResult(result);
    }

}
