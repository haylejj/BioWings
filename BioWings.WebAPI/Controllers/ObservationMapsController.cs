using BioWings.Application.Features.Queries.ObservationMapQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
public class ObservationMapsController(IMediator mediator, ILogger<ObservationMapsController> logger) : BaseController
{
    // GET: api/ObservationMaps
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] double minLat, [FromQuery] double maxLat, [FromQuery] double minLng, [FromQuery] double maxLng, [FromQuery] int zoomLevel)
    {
        var result = await mediator.Send(new ObservationMapGetQuery { MaxLat=maxLat, MinLat=minLat, MaxLng=maxLng, MinLng=minLng, ZoomLevel=zoomLevel });
        logger.LogInformation($"Received query parameters: MinLat={minLat}, MaxLat={maxLat}, MinLng={minLng}, MaxLng={maxLng}, ZoomLevel={zoomLevel}");
        return CreateResult(result);
    }
    // GET: api/ObservationMaps/Id
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await mediator.Send(new ObservationMapGetByObservationIdQuery(id));
        return CreateResult(result);
    }
}