using BioWings.Application.Features.Queries.StatisticsQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class StatisticsController(IMediator mediator) : BaseController
{
    // GET : api/Statistics
    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
        var query = new StatisticsGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
}
