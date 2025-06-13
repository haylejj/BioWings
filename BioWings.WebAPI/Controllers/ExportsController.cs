using BioWings.Application.Features.Commands.ExportCommands;
using BioWings.Application.Features.Queries.ExportQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ExportsController(IMediator mediator) : BaseController
{
    // GET: api/Exports/GetColumnNames
    [HttpGet("GetColumnNames")]
    [AuthorizeDefinition("Veri Dışa Aktarma", ActionType.Read, "Export kolon isimlerini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetColumnNames()
    {
        var query = new ExportGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // POST: api/Exports/ExportData
    [HttpPost("ExportData")]
    [AuthorizeDefinition("Veri Dışa Aktarma", ActionType.Write, "Veri dışa aktarma işlemi", AreaNames.Public)]
    public async Task<IActionResult> ExportData([FromBody] ExportCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // GET: api/Exports/ExportObservationsByProvince
    [HttpGet("ExportObservationsByProvince/{code}")]
    [AuthorizeDefinition("Veri Dışa Aktarma", ActionType.Write, "İle göre gözlem dışa aktarma", AreaNames.Public)]
    public async Task<IActionResult> ExportObservationsByProvince([FromRoute] int code)
    {
        var result = await mediator.Send(new ExportObservationsByProvinceQuery(code));
        return CreateResult(result);
    }
}
