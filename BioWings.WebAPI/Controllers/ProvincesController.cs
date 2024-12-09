using BioWings.Application.Features.Commands.ProvinceCommands;
using BioWings.Application.Features.Queries.ProvinceQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ProvincesController(IMediator mediator) : BaseController
{
    // GET: api/Provinces
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new ProvinceGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // GET: api/Provinces/Paged
    [HttpGet("Paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var query = new ProvinceGetPagedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // GET: api/Provinces/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new ProvinceGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }

    // POST: api/Provinces
    [HttpPost]
    public async Task<IActionResult> Create(ProvinceCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    // POST: api/Provinces/Range
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(ProvinceCreateRangeCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }


    // PUT: api/Provinces
    [HttpPut]
    public async Task<IActionResult> Update(ProvinceUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Provinces/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new ProvinceRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
}
