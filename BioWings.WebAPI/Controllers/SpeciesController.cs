using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Features.Queries.SpeciesQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class SpeciesController(IMediator mediator) : BaseController
{
    // GET: api/Species
    [AuthorizeDefinition("Species",ActionType.Read, "Tüm Türleri Getir",AreaNames.Public)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new SpeciesGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Species/Paged
    [AuthorizeDefinition("Species", ActionType.Read, "Tüm Türleri Sayfalama İle Getir", AreaNames.Public)]
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new SpeciesGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Species/Search
    [AuthorizeDefinition("Species", ActionType.Read, "Türleri Search İle Getir", AreaNames.Public)]
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new SpeciesSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

    // GET: api/Species/{id}
    [AuthorizeDefinition("Species", ActionType.Read, "Türü Id'ye göre getir.", AreaNames.Public)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new SpeciesGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // POST: api/Species
    [AuthorizeDefinition("Species", ActionType.Write, "Tür Ekleme", AreaNames.Public)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SpeciesCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Species/Range
    [AuthorizeDefinition("Species", ActionType.Write, "Birden Fazla Tür Ekleme", AreaNames.Public)]
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(SpeciesCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Species/Import
    [AuthorizeDefinition("Species", ActionType.Write, "Import ile Tür Ekleme", AreaNames.Public)]
    [HttpPost("Import")]
    public async Task<IActionResult> Import(SpeciesImportCreateCommand speciesImportCreateCommand)
    {
        var result = await mediator.Send(speciesImportCreateCommand);
        return CreateResult(result);
    }

    // PUT: api/Species
    [AuthorizeDefinition("Species", ActionType.Update, "Tür Güncelleme", AreaNames.Public)]
    [HttpPut]
    public async Task<IActionResult> Update(SpeciesUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Species/{id}
    [AuthorizeDefinition("Species", ActionType.Delete, "Tür Silme", AreaNames.Public)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new SpeciesRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
