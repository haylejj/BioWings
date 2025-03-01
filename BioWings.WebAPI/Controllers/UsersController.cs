using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Features.Queries.UserQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class UsersController(IMediator mediator) : BaseController
{
    // GET: api/Users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new UserGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Users/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new UserGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new UserGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Users
    [HttpPost]
    public async Task<IActionResult> Create(UserCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    [HttpPost("ByAdmin")]
    public async Task<IActionResult> CreateByAdmin(UserCreateByAdminCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // PUT: api/Users
    [HttpPut]
    public async Task<IActionResult> Update(UserUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new UserRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
