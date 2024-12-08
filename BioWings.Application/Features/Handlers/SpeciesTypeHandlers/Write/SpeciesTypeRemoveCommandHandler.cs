using BioWings.Application.Features.Commands.SpeciesTypeCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Write;

public class SpeciesTypeRemoveCommandHandler(ISpeciesTypeRepository speciesTypeRepository, IUnitOfWork unitOfWork, ILogger<SpeciesTypeRemoveCommandHandler> logger) : IRequestHandler<SpeciesTypeRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesTypeRemoveCommand request, CancellationToken cancellationToken)
    {
        var speciesType = await speciesTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciesType is null)
        {
            logger.LogWarning("SpeciesType {Id} not found", request.Id);
            return ServiceResult.Error("SpeciesType not found", HttpStatusCode.NotFound);
        }
        speciesTypeRepository.Remove(speciesType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("SpeciesType {Id} removed", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
