using BioWings.Application.Features.Commands.GenusCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Write;
public class GenusUpdateCommandHandler(IGenusRepository genusRepository, ILogger<GenusUpdateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<GenusUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(GenusUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("GenusUpdateCommand is null");
            return ServiceResult.Error("GenusUpdateCommand is null");
        }
        var genus = await genusRepository.GetByIdAsync(request.Id);
        if (genus == null)
        {
            logger.LogError("Genus not found with ID: {GenusId}", request.Id);
            return ServiceResult.Error("Genus not found", System.Net.HttpStatusCode.NotFound);
        }
        genus.Name = request.Name;
        genus.FamilyId = request.FamilyId;
        genusRepository.Update(genus);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Genus updated successfully with ID: {GenusId}", genus.Id);
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
