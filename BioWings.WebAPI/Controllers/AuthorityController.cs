using BioWings.Application.Features.Commands.AuthorityCommands;
using BioWings.Application.Features.Queries.AuthorityQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class AuthorityController(IMediator mediator) : BaseController
{
    // GET: api/Authorities
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new AuthorityGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Authorities/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new AuthorityGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Authorities/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new AuthorityGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Authorities
    [HttpPost]
    public async Task<IActionResult> Create(AuthorityCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Authorities/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(AuthorityCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Authorities
    [HttpPut]
    public async Task<IActionResult> Update(AuthorityUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Authorities/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new AuthorityRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
