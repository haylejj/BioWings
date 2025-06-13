using BioWings.Application.Features.Commands.ProvinceCommands;
using BioWings.Application.Features.Queries.ProvinceQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ProvincesController(IMediator mediator) : BaseController
{
    // GET: api/Provinces
    [HttpGet]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Read, "Tüm illeri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new ProvinceGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Provinces/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Read, "Sayfalı il listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ProvinceGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Provinces/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Read, "İl detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new ProvinceGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Provinces
    [HttpPost]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Write, "Yeni il oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(ProvinceCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Provinces/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Write, "Birden fazla il oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(ProvinceCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Provinces
    [HttpPut]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Update, "İl güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(ProvinceUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Provinces/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("İl Yönetimi", ActionType.Delete, "İl silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new ProvinceRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
