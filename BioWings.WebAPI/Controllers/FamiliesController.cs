using BioWings.Application.Features.Commands.FamilyCommands;
using BioWings.Application.Features.Queries.FamilyQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class FamiliesController(IMediator mediator) : BaseController
{
    // GET: api/Families
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new FamilyGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Families/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new FamilyGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Families/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new FamilyGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Families
    [HttpPost]
    public async Task<IActionResult> Create(FamilyCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Families/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(FamilyCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Families
    [HttpPut]
    public async Task<IActionResult> Update(FamilyUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Families/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new FamilyRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
