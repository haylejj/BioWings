using BioWings.Application.Features.Commands.GenusCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Write;
public class GenusRemoveCommandHandler(IGenusRepository genusRepository, ILogger<GenusRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<GenusRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(GenusRemoveCommand request, CancellationToken cancellationToken)
    {
        var genus = await genusRepository.GetByIdAsync(request.Id);
        if (genus == null)
        {
            logger.LogWarning("Genus not found with ID: {GenusId}", request.Id);
            return ServiceResult.Error("Genus not found", HttpStatusCode.NotFound);
        }
        genusRepository.Remove(genus);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Genus removed successfully with ID: {GenusId}", genus.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}

