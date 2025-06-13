using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ObserversController(IMediator mediator) : BaseController
{
    // GET: api/Observers
    [HttpGet]
    [AuthorizeDefinition("Gözlemci Yönetimi", ActionType.Read, "Tüm gözlemcileri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new ObserverGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observers/Paged
    [HttpGet("Paged")]
    [AuthorizeDefinition("Gözlemci Yönetimi", ActionType.Read, "Sayfalı gözlemci listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ObserverGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observers/Search
    [HttpGet("Search")]
    [AuthorizeDefinition("Gözlemci Yönetimi", ActionType.Read, "Gözlemci arama", AreaNames.Public)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new ObserverSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

    // GET: api/Observers/{id}
    [HttpGet("{id}")]
    [AuthorizeDefinition("Gözlemci Yönetimi", ActionType.Read, "Gözlemci detayını görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new ObserverGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observers/{id}/Observations
    [HttpGet("{id}/Observations")]
    [AuthorizeDefinition("Gözlemci Yönetimi", ActionType.Read, "Gözlemci gözlemlerini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetObservations(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ObserverGetObservationsWithPagedQuery { ObserverId=id, PageNumber=pageNumber, PageSize=pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observers/{id}/Observations/Search
    [HttpGet("{id}/Observations/Search")]
    [AuthorizeDefinition("Gözlemci Yönetimi", ActionType.Read, "Gözlemci gözlemlerinde arama", AreaNames.Public)]
    public async Task<IActionResult> SearchObservations(int id, [FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new ObserverGetObservationsWithSearchQuery { ObserverId=id, PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

}
