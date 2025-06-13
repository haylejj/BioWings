using BioWings.Application.Features.Commands.AuthorityCommands;
using BioWings.Application.Features.Queries.AuthorityQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class AuthoritiesController(IMediator mediator) : BaseController
{
    // GET: api/Authorities
    [HttpGet]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Read, "Tüm otoriteleri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new AuthorityGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Authorities/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Read, "Sayfalı otorite listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new AuthorityGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Authorities/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Read, "Otorite detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new AuthorityGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Authorities
    [HttpPost]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Write, "Yeni otorite oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(AuthorityCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Authorities/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Write, "Birden fazla otorite oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(AuthorityCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Authorities
    [HttpPut]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Update, "Otorite güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(AuthorityUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Authorities/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Otorite Yönetimi", ActionType.Delete, "Otorite silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new AuthorityRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
