using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
[EnableCors("AllowMvcApp")]
public class ObservationsController(IMediator mediator) : BaseController
{
    // GET: api/Observations
    [HttpGet]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "Tüm gözlemleri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new ObservationGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observations/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "Sayfalı gözlem listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ObservationGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Observations/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "Gözlem detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new ObservationGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // POST: api/Observations
    [HttpPost]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Write, "Yeni gözlem oluşturma", AreaNames.Public)]
    public async Task<IActionResult> Create([FromBody] ObservationCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Observations/Range
    [HttpPost("Range")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Write, "Birden fazla gözlem oluşturma", AreaNames.Public)]
    public async Task<IActionResult> CreateRange([FromBody] ObservationCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // PUT: api/Observations
    [HttpPut]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Update, "Gözlem güncelleme", AreaNames.Public)]
    public async Task<IActionResult> Update([FromBody] ObservationUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // DELETE: api/Observations/{id}
    [HttpDelete("{id}")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Delete, "Gözlem silme", AreaNames.Public)]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new ObservationRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Observations/Import
    [HttpPost("Import")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Write, "Gözlem import etme", AreaNames.Admin)]
    public async Task<IActionResult> Import([FromForm] ObservationImportCreateCommand command)
    {
        var reuslt = await mediator.Send(command);
        return CreateResult(reuslt);
    }
    // Post: api/Observations/ImportAllFormat
    [HttpPost("ImportAllFormat")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Write, "Tüm formatlarla gözlem import etme", AreaNames.Admin)]
    public async Task<IActionResult> ImportAllFormat([FromForm] ObservationImportAllFormatCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // GET: api/Observations/Search
    [HttpGet("Search")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "Gözlem arama", AreaNames.Public)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new ObservationSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }
    // GET: api/Observations/Filter
    [HttpGet("Filter")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "Gözlem filtreleme", AreaNames.Public)]
    public async Task<IActionResult> GetFiltered([FromQuery] List<string> columnNames, [FromQuery] List<string> columnValues, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ObservationGetFilteredQuery { ColumnNames=columnNames, ColumnValues=columnValues, PageNumber=pageNumber, PageSize=pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observations/Count
    [HttpGet("Count")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "Gözlem sayısı görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetCount()
    {
        var query = new ObservationGetCountQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observations/ByProvince/{provinceCode}
    [HttpGet("ByProvince/{provinceCode}")]
    [AuthorizeDefinition("Gözlem Yönetimi", ActionType.Read, "İle göre gözlem görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetByProvince(int provinceCode, int pageNumber = 1, int pageSize = 25)
    {
        var query = new ObservationGetByProvinceQuery(provinceCode, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
}
