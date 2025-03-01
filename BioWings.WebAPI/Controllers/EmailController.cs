using BioWings.Application.Features.Commands.EmailCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class EmailController(IMediator mediator) : BaseController
{
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(EmailConfirmCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    [HttpPost("resetPasswordLink")]
    public async Task<IActionResult> ResetPasswordLink(ResetPasswordLinkCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    [HttpPost("resendEmailConfirmation")]
    public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmMailCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
