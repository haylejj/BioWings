using BioWings.Application.Features.Commands.UserCommands;
using BioWings.Application.Features.Queries.UserQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class UsersController(IMediator mediator) : BaseController
{
    // GET: api/Users
    [HttpGet]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Read, "Kullanıcı listesini görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetAll()
    {
        var query = new UserGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Users/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Read, "Sayfalı kullanıcı listesini görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new UserGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Users/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Read, "Kullanıcı detayını görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new UserGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Users
    [HttpPost]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Write, "Yeni kullanıcı oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(UserCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    [HttpPost("ByAdmin")]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Write, "Admin tarafından kullanıcı oluşturma", AreaNames.Admin)]
    public async Task<IActionResult> CreateByAdmin(UserCreateByAdminCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // PUT: api/Users
    [HttpPut]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Update, "Kullanıcı bilgilerini güncelleme", AreaNames.Admin)]
    public async Task<IActionResult> Update(UserUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Users/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Delete, "Kullanıcı silme", AreaNames.Admin)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new UserRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
