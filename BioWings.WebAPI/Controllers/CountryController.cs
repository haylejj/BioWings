using BioWings.Application.Features.Commands.CountryCommands;
using BioWings.Application.Features.Queries.CountryQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class CountryController(IMediator mediator) : BaseController
{
    [HttpGet]
    [AuthorizeDefinition("Ülke Yönetimi", ActionType.Read, "Tüm ülkeleri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> Get()
    {
        var query = new CountryGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    [HttpPost]
    [AuthorizeDefinition("Ülke Yönetimi", ActionType.Write, "Yeni ülke oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(CountryCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    [HttpPost("Range")]
    [AuthorizeDefinition("Ülke Yönetimi", ActionType.Write, "Birden fazla ülke oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(CountryCreateRangeCommand countryCreateRangeCommand)
    {
        var result = await mediator.Send(countryCreateRangeCommand);
        return CreateResult(result);
    }
    // GET: api/Country/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Ülke Yönetimi", ActionType.Read, "Ülke detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new CountryGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // PUT: api/Country
    [HttpPut]
    [AuthorizeDefinition("Ülke Yönetimi", ActionType.Update, "Ülke güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(CountryUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Country/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Ülke Yönetimi", ActionType.Delete, "Ülke silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new CountryRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

}
