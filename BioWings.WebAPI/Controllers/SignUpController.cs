using BioWings.Application.Features.Commands.SignUpCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class SignUpController(IMediator mediator) : BaseController
{
    // POST: api/SignUp
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
