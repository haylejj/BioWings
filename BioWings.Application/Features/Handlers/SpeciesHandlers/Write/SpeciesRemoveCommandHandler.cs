using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesRemoveCommandHandler(ISpeciesRepository speciesRepository, IUnitOfWork unitOfWork, ILogger<SpeciesRemoveCommandHandler> logger) : IRequestHandler<SpeciesRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesRemoveCommand request, CancellationToken cancellationToken)
    {
        var species = await speciesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (species == null)
        {
            logger.LogError("Species is not found that has id:{0}", request.Id);
            return ServiceResult.Error($"Species is not found that has id: {request.Id}", System.Net.HttpStatusCode.NotFound);
        }
        speciesRepository.Remove(species);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Species is removed that has id:{0}", request.Id);
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
