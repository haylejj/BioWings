using BioWings.Application.Features.Commands.LocationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Write;

public class LocationCreateRangeCommandHandler(ILocationRepository locationRepository, ILogger<LocationCreateRangeCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<LocationCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(LocationCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request == null || !request.LocationCreateCommands.Any())
        {
            logger.LogWarning("LocationCreateRangeCommand is null");
            return ServiceResult.Error("LocationCreateRangeCommand is null", HttpStatusCode.BadRequest);
        }
        var locations = request.LocationCreateCommands.Select(x => new Location
        {
            ProvinceId = x.ProvinceId,
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
            CoordinatePrecisionLevel = x.CoordinatePrecisionLevel
        });
        await locationRepository.AddRangeAsync(locations, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Locations created");
        return ServiceResult.SuccessAsCreated("api/Locations/Range");
    }
}