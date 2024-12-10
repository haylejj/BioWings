using BioWings.Application.Features.Commands.LocationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Write;
public class LocationUpdateCommandHandler(ILocationRepository locationRepository, ILogger<LocationUpdateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<LocationUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(LocationUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("LocationUpdateCommand is null");
            return ServiceResult.Error("LocationUpdateCommand is null", HttpStatusCode.BadRequest);
        }
        var location = await locationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (location == null)
        {
            logger.LogWarning("Location not found");
            return ServiceResult.Error("Location not found", HttpStatusCode.NotFound);
        }
        location.ProvinceId = request.ProvinceId;
        location.Name = request.Name;
        location.SquareRef = request.SquareRef;
        location.Latitude = request.Latitude;
        location.Longitude = request.Longitude;
        location.Altitude1 = request.Altitude1;
        location.Altitude2 = request.Altitude2;
        location.UtmReference = request.UtmReference;
        location.Description = request.Description;
        location.DecimalMinutes = request.DecimalMinutes;
        location.DecimalDegrees = request.DecimalDegrees;
        location.DegreesMinutesSeconds = request.DegreesMinutesSeconds;
        location.UtmCoordinates = request.UtmCoordinates;
        location.MgrsCoordinates = request.MgrsCoordinates;
        location.SquareLatitude = request.SquareLatitude;
        location.SquareLongitude = request.SquareLongitude;
        location.CoordinatePrecisionLevel = request.CoordinatePrecisionLevel;
        locationRepository.Update(location);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Location Id : {0} updated successfully.", location.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
