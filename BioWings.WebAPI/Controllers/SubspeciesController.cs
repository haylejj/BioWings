using BioWings.Application.Features.Commands.SubspeciesCommands;
using BioWings.Application.Features.Queries.SubspeciesQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class SubspeciesController(IMediator mediator) : BaseController
{
    // GET: api/Subspecies
    [HttpGet]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Read, "Tüm alt türleri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new SubspeciesGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Subspecies/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Read, "Sayfalı alt tür listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new SubspeciesGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Subspecies/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Read, "Alt tür detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new SubspeciesGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Subspecies
    [HttpPost]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Write, "Yeni alt tür oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(SubspeciesCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Subspecies/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Write, "Birden fazla alt tür oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(SubspeciesCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Subspecies
    [HttpPut]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Update, "Alt tür güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(SubspeciesUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Subspecies/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Alt Tür Yönetimi", ActionType.Delete, "Alt tür silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new SubspeciesRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
