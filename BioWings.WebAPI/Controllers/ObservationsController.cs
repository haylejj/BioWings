using BioWings.Application.Features.Commands.ObservationCommands;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
[EnableCors("AllowMvcApp")]
public class ObservationsController(IMediator mediator) : BaseController
{
    // POST: api/Observations
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ObservationCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Observations/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange([FromBody] ObservationCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Observations/Import
    [HttpPost("Import")]
    public async Task<IActionResult> Import([FromForm] ObservationImportCreateCommand command)
    {
        var reuslt = await mediator.Send(command);
        return CreateResult(reuslt);
    }
}
