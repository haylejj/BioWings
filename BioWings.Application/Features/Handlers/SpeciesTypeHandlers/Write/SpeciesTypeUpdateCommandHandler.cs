using BioWings.Application.Features.Commands.SpeciesTypeCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Write;

public class SpeciesTypeUpdateCommandHandler(ISpeciesTypeRepository speciesTypeRepository, IUnitOfWork unitOfWork, ILogger<SpeciesTypeUpdateCommandHandler> logger) : IRequestHandler<SpeciesTypeUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesTypeUpdateCommand request, CancellationToken cancellationToken)
    {
        var speciesType = await speciesTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciesType is null)
        {
            logger.LogWarning("SpeciesType {Id} not found", request.Id);
            return ServiceResult.Error("SpeciesType not found", HttpStatusCode.NotFound);
        }
        speciesType.Name = request.Name;
        speciesType.Description = request.Description;
        speciesTypeRepository.Update(speciesType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("SpeciesType {Id} updated", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
