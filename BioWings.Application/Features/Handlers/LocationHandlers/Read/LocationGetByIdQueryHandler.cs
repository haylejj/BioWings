using BioWings.Application.Features.Queries.LocationQueries;
using BioWings.Application.Features.Results.LocationResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Read;
public class LocationGetByIdQueryHandler(ILocationRepository locationRepository, ILogger<LocationGetByIdQueryHandler> logger) : IRequestHandler<LocationGetByIdQuery, ServiceResult<LocationGetByIdQueryResult>>
{
    public async Task<ServiceResult<LocationGetByIdQueryResult>> Handle(LocationGetByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetByIdWithProvinceAsync(request.Id, cancellationToken);
        if (location == null)
        {
            logger.LogWarning("Location not found");
            return ServiceResult<LocationGetByIdQueryResult>.Error("Location not found", HttpStatusCode.NotFound);
        }
        var result = new LocationGetByIdQueryResult
        {
            Id = location.Id,
            ProvinceId = location.ProvinceId,
            ProvinceName = location.Province.Name,
            Name = location.Name,
            SquareRef = location.SquareRef,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Altitude1 = location.Altitude1,
            Altitude2 = location.Altitude2,
            UtmReference = location.UtmReference,
            Description = location.Description,
            DecimalDegrees = location.DecimalDegrees,
            DegreesMinutesSeconds = location.DegreesMinutesSeconds,
            DecimalMinutes = location.DecimalMinutes,
            UtmCoordinates = location.UtmCoordinates,
            MgrsCoordinates = location.MgrsCoordinates,
            SquareLatitude = location.SquareLatitude,
            SquareLongitude = location.SquareLongitude,
            CoordinatePrecisionLevel = location.CoordinatePrecisionLevel
        };
        logger.LogInformation("Location found Id:{0} ", result.Id);
        return ServiceResult<LocationGetByIdQueryResult>.Success(result);
    }
}
