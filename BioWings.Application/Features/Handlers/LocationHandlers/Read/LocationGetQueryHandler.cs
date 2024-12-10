using BioWings.Application.Features.Queries.LocationQueries;
using BioWings.Application.Features.Results.LocationResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Read;
public class LocationGetQueryHandler(ILocationRepository locationRepository, ILogger<LocationGetQueryHandler> logger) : IRequestHandler<LocationGetQuery, ServiceResult<IEnumerable<LocationGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<LocationGetQueryResult>>> Handle(LocationGetQuery request, CancellationToken cancellationToken)
    {
        var locations = await locationRepository.GetAllAsQueryable().Include(x => x.Province).ToListAsync(cancellationToken);
        var result = locations.Select(x => new LocationGetQueryResult
        {
            Id = x.Id,
            ProvinceId = x.ProvinceId,
            ProvinceName = x.Province.Name,
            SquareRef = x.SquareRef,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
            Altitude1 = x.Altitude1,
            Altitude2 = x.Altitude2,
            UtmReference = x.UtmReference,
            Description = x.Description,
            DecimalDegrees = x.DecimalDegrees,
            DegreesMinutesSeconds = x.DegreesMinutesSeconds,
            DecimalMinutes = x.DecimalMinutes,
            UtmCoordinates = x.UtmCoordinates,
            MgrsCoordinates = x.MgrsCoordinates,
            SquareLatitude = x.SquareLatitude,
            SquareLongitude = x.SquareLongitude,
            CoordinatePrecisionLevel=x.CoordinatePrecisionLevel
        });
        logger.LogInformation("Locations found");
        return ServiceResult<IEnumerable<LocationGetQueryResult>>.Success(result);
    }
}
