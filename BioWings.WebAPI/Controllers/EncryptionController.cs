using BioWings.Application.Features.Commands.EncryptionCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class EncryptionController(IMediator mediator) : BaseController
{
    [HttpPost("decrypt")]
    public async Task<IActionResult> Decrypt([FromBody] DecryptCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
