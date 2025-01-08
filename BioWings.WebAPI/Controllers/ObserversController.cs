using BioWings.Application.Features.Queries.ObserverQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ObserversController(IMediator mediator) : BaseController
{
    // GET: api/Observers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new ObserverGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observers/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ObserverGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Observers/Search
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var searchQuery = new ObserverSearchQuery { PageNumber=pageNumber, PageSize=pageSize, SearchTerm=searchTerm };
        var result = await mediator.Send(searchQuery);
        return CreateResult(result);
    }

    // GET: api/Observers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new ObserverGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

}
