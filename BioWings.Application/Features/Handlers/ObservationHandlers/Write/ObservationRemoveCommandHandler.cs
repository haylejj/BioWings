using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationRemoveCommandHandler(IObservationRepository observationRepository, IUnitOfWork unitOfWork, ILogger<ObservationRemoveCommandHandler> logger) : IRequestHandler<ObservationRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationRemoveCommand request, CancellationToken cancellationToken)
    {
        var observation = await observationRepository.GetByIdAsync(request.Id);
        if (observation == null)
        {
            logger.LogError($"Observation with id {request.Id} not found");
            return ServiceResult.Error($"Observation with id {request.Id} not found", System.Net.HttpStatusCode.NotFound);
        }
        observationRepository.Remove(observation);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation($"Observation with id {request.Id} removed successfully");
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
