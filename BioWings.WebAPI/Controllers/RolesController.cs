using BioWings.Application.Features.Commands.RoleCommands;
using BioWings.Application.Features.Queries.RoleQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class RolesController(IMediator mediator) : BaseController
{
    // GET: api/Roles
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new RoleGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Roles/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new RoleGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // POST: api/Roles
    [HttpPost]
    public async Task<IActionResult> Create(RoleCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Roles/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(RoleCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // PUT: api/Roles
    [HttpPut]
    public async Task<IActionResult> Update(RoleUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // DELETE: api/Roles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new RoleRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
