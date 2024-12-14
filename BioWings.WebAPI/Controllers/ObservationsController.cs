using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Features.Queries.ObservationQueries;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
[EnableCors("AllowMvcApp")]
public class ObservationsController(IMediator mediator) : BaseController
{
    // GET: api/Observations
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new ObservationGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observations/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ObservationGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Observations/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new ObservationGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
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
