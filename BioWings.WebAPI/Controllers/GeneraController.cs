using BioWings.Application.Features.Commands.GenusCommands;
using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Queries.ObservationQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class GeneraController(IMediator mediator) : BaseController
{
    // GET: api/Genera
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GenusGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Genera/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new GenusGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Genera/Search
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new GenusSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

    // GET: api/Genera/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GenusGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Genera
    [HttpPost]
    public async Task<IActionResult> Create(GenusCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Genera/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(GenusCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Genera
    [HttpPut]
    public async Task<IActionResult> Update(GenusUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Genera/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new GenusRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
