using BioWings.Application.Features.Commands.LocationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Write;

public class LocationRemoveCommandHandler(ILocationRepository locationRepository, ILogger<LocationRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<LocationRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(LocationRemoveCommand request, CancellationToken cancellationToken)
    {
        var location=await locationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (location == null)
        {
            logger.LogWarning("Location not found");
            return ServiceResult.Error("Location not found", HttpStatusCode.NotFound);
        }
        locationRepository.Remove(location);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Location Id : {0} removed successfully.", location.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
