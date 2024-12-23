using BioWings.Application.Features.Commands.SubspeciesCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Write;
public class SubspeciesRemoveCommandHandler(ISubspeciesRepository speciesTypeRepository, IUnitOfWork unitOfWork, ILogger<SubspeciesRemoveCommandHandler> logger) : IRequestHandler<SubspeciesRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SubspeciesRemoveCommand request, CancellationToken cancellationToken)
    {
        var speciesType = await speciesTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciesType == null)
        {
            logger.LogWarning("SubspeciesRemoveCommandHandler: SpeciesType not found");
            return ServiceResult.Error("SpeciesType not found");
        }
        speciesTypeRepository.Remove(speciesType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("SpeciesType Id : {0} removed successfully", speciesType.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
