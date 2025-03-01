using BioWings.Application.Features.Commands.CountryCommands;
using BioWings.Application.Features.Queries.CountryQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;

public class CountryController(IMediator mediator) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new CountryGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CountryCreateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }
    [HttpPost("Range")]
    public async Task<IActionResult> CreateRange(CountryCreateRangeCommand countryCreateRangeCommand)
    {
        var result = await mediator.Send(countryCreateRangeCommand);
        return CreateResult(result);
    }
    // GET: api/Country/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new CountryGetByIdQuery(id);
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
    // PUT: api/Country
    [HttpPut]
    public async Task<IActionResult> Update(CountryUpdateCommand command)
    {
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

    // DELETE: api/Country/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var command = new CountryRemoveCommand(id);
        var result = await mediator.Send(command);
        return CreateResult(result);
    }

}
