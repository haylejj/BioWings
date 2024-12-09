using BioWings.Application.Features.Commands.SubspeciesCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Write;
public class SubspeciesUpdateCommandHandler(ISubspeciesRepository subspeciesRepository, IUnitOfWork unitOfWork, ILogger<SubspeciesRemoveCommandHandler> logger) : IRequestHandler<SubspeciesUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SubspeciesUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("SubspeciesUpdateCommandHandler: request is null");
            return ServiceResult.Error("Request is null");
        }
        var subSpeciesType = await subspeciesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (subSpeciesType == null)
        {
            logger.LogWarning("SubspeciesUpdateCommandHandler: SpeciesType not found");
            return ServiceResult.Error("SpeciesType not found");
        }
        subSpeciesType.Name = request.Name;
        subSpeciesType.SpeciesId = request.SpeciesId;
        subspeciesRepository.Update(subSpeciesType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Subspecies Id : {0} updated successfully", subSpeciesType.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
