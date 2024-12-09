using BioWings.Application.Features.Commands.LocationCommands;
using BioWings.Application.Features.Queries.LocationQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class LocationsController(IMediator mediator) : BaseController
{
    // GET: api/Locations
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new LocationGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Locations/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new LocationGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Locations/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new LocationGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Locations
    [HttpPost]
    public async Task<IActionResult> Create(LocationCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Locations/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(LocationCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Locations
    [HttpPut]
    public async Task<IActionResult> Update(LocationUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Locations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new LocationRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
