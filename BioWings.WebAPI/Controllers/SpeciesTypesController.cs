using BioWings.Application.Features.Commands.SpeciesTypeCommands;
using BioWings.Application.Features.Queries.SpeciesTypeQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class SpeciesTypesController(IMediator mediator) : BaseController
{
    // GET: api/SpeciesTypes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new SpeciesTypeGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/SpeciesTypes/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new SpeciesTypeGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/SpeciesTypes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new SpeciesTypeGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/SpeciesTypes
    [HttpPost]
    public async Task<IActionResult> Create(SpeciesTypeCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/SpeciesTypes/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(SpeciesTypeCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/SpeciesTypes
    [HttpPut]
    public async Task<IActionResult> Update(SpeciesTypeUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/SpeciesTypes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new SpeciesTypeRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
