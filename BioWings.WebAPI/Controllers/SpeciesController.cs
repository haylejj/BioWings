using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Features.Queries.SpeciesQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class SpeciesController(IMediator mediator) : BaseController
{
    // GET: api/Species
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new SpeciesGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Species/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new SpeciesGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Species/Search
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new SpeciesSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

    // GET: api/Species/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new SpeciesGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Species
    [HttpPost]
    public async Task<IActionResult> Create(SpeciesCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Species/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(SpeciesCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Species
    [HttpPut]
    public async Task<IActionResult> Update(SpeciesUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Species/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new SpeciesRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
