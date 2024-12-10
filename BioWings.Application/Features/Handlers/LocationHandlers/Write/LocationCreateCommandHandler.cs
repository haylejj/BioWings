using BioWings.Application.Features.Commands.LocationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Write;
public class LocationCreateCommandHandler(ILocationRepository locationRepository, ILogger<LocationCreateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<LocationCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(LocationCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("LocationCreateCommand is null");
            return ServiceResult.Error("LocationCreateCommand is null", HttpStatusCode.BadRequest);
        }
        var location = new Location
        {
            ProvinceId = request.ProvinceId,
            SquareRef = request.SquareRef,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Altitude1 = request.Altitude1,
            Altitude2 = request.Altitude2,
            UtmReference = request.UtmReference,
            Description = request.Description,
            DecimalMinutes = request.DecimalMinutes,
            DecimalDegrees = request.DecimalDegrees,
            DegreesMinutesSeconds = request.DegreesMinutesSeconds,
            UtmCoordinates = request.UtmCoordinates,
            MgrsCoordinates = request.MgrsCoordinates,
            SquareLatitude = request.SquareLatitude,
            SquareLongitude = request.SquareLongitude,
            CoordinatePrecisionLevel = request.CoordinatePrecisionLevel
        };
        await locationRepository.AddAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Location created Id:{0}", location.Id);
        return ServiceResult.SuccessAsCreated("api/Locations/"+location.Id);
    }
}
