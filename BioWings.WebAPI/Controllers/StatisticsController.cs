using BioWings.Application.Features.Queries.StatisticsQueries;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class StatisticsController(IMediator mediator) : BaseController
{
    // GET : api/Statistics
    [HttpGet]
    [AuthorizeDefinition("İstatistikler", ActionType.Read, "İstatistikleri görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetStatistics()
    {
        var query = new StatisticsGetQuery();
        var result = await mediator.Send(query);
        return CreateResult(result);
    }
}
