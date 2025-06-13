using BioWings.Application.Features.Commands.GenusCommands;
using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class GeneraController(IMediator mediator) : BaseController
{
    // GET: api/Genera
    [HttpGet]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Read, "Tüm genusları görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new GenusGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Genera/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Read, "Sayfalı genus listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new GenusGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Genera/Search
    [HttpGet("Search")]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Read, "Genus arama", AreaNames.Public)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new GenusSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }
    // GET: api/Genera/Count
    [HttpGet("Count")]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Read, "Genus sayısını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> Count()
    {
        var query = new GenusGetCountQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Genera/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Read, "Genus detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GenusGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Genera
    [HttpPost]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Write, "Yeni genus oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(GenusCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Genera/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Write, "Birden fazla genus oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(GenusCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Genera
    [HttpPut]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Update, "Genus güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(GenusUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Genera/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Genus Yönetimi", ActionType.Delete, "Genus silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new GenusRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
