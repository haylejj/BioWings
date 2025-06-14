using BioWings.Application.Features.Commands.RoleCommands;
using BioWings.Application.Features.Queries.RoleQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class RolesController(IMediator mediator) : BaseController
{
    // GET: api/Roles
    [HttpGet]
    [AuthorizeDefinition("Rol Yönetimi", ActionType.Read, "Tüm rolleri görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetAll()
    {
        var query = new RoleGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Roles/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Rol Yönetimi", ActionType.Read, "Rol detayını görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new RoleGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // POST: api/Roles
    [HttpPost]
    [AuthorizeDefinition("Rol Yönetimi", ActionType.Write, "Yeni rol oluşturma", AreaNames.Admin)]
    public async Task<IActionResult> Create(RoleCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Roles/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Rol Yönetimi", ActionType.Write, "Birden fazla rol oluşturma", AreaNames.Admin)]
    public async Task<IActionResult> CreateRange(RoleCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // PUT: api/Roles
    [HttpPut]
    [AuthorizeDefinition("Rol Yönetimi", ActionType.Update, "Rol güncelleme", AreaNames.Admin)]
    public async Task<IActionResult> Update(RoleUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // DELETE: api/Roles/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Rol Yönetimi", ActionType.Delete, "Rol silme", AreaNames.Admin)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new RoleRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
