using BioWings.Application.Features.Commands.ExportCommands;
using BioWings.Application.Features.Queries.ExportQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ExportsController(IMediator mediator) : BaseController
{
    // GET: api/Exports/GetColumnNames
    [HttpGet("GetColumnNames")]
    public async Task<IActionResult> GetColumnNames()
    {
        var query = new ExportGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // POST: api/Exports/ExportData
    [HttpPost("ExportData")]
    public async Task<IActionResult> ExportData([FromBody] ExportCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
