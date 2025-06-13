using BioWings.Application.Features.Commands.LoginCommands;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class LoginController(IMediator mediator) : BaseController
{
    [HttpPost]
    [AuthorizeDefinition("Kimlik Doğrulama", ActionType.Write, "Kullanıcı giriş işlemi", AreaNames.Public)]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

}
