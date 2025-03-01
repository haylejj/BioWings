using BioWings.Application.Features.Commands.LoginCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class LoginController(IMediator mediator) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

}
