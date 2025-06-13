using BioWings.Application.Features.Commands.FamilyCommands;
using BioWings.Application.Features.Queries.FamilyQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class FamiliesController(IMediator mediator) : BaseController
{
    // GET: api/Families
    [HttpGet]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Read, "Tüm familyaları görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new FamilyGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Families/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Read, "Sayfalı familya listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new FamilyGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Families/Search
    [HttpGet("Search")]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Read, "Familya arama", AreaNames.Public)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new FamilySearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

    // GET: api/Families/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Read, "Familya detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new FamilyGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Families
    [HttpPost]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Write, "Yeni familya oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create(FamilyCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Families/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Write, "Birden fazla familya oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange(FamilyCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Families
    [HttpPut]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Update, "Familya güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update(FamilyUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Families/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Familya Yönetimi", ActionType.Delete, "Familya silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new FamilyRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
