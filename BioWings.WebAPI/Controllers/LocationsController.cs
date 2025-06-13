using BioWings.Application.Features.Commands.LocationCommands;
using BioWings.Application.Features.Queries.LocationQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class LocationsController(IMediator mediator) : BaseController
{
    // GET: api/Locations
    [HttpGet]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Read, "Tüm lokasyonları görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new LocationGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Locations/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Read, "Sayfalı lokasyon listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new LocationGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Locations/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Read, "Lokasyon detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new LocationGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Locations
    [HttpPost]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Write, "Yeni lokasyon oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(LocationCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Locations/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Write, "Birden fazla lokasyon oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(LocationCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Locations
    [HttpPut]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Update, "Lokasyon güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(LocationUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Locations/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Lokasyon Yönetimi", ActionType.Delete, "Lokasyon silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new LocationRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
