using BioWings.Application.Features.Commands.SubspeciesCommands;
using BioWings.Application.Features.Queries.SubspeciesQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class SubspeciesController(IMediator mediator) : BaseController
{
    // GET: api/Subspecies
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new SubspeciesGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Subspecies/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new SubspeciesGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Subspecies/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new SubspeciesGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Subspecies
    [HttpPost]
    public async Task<IActionResult> Create(SubspeciesCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Subspecies/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(SubspeciesCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Subspecies
    [HttpPut]
    public async Task<IActionResult> Update(SubspeciesUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Subspecies/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new SubspeciesRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
