using BioWings.Application.Features.Commands.GenusCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Write;
public class GenusCreateCommandHandler(IGenusRepository genusRepository, ILogger<GenusCreateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<GenusCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(GenusCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("GenusCreateCommand is null");
            return ServiceResult.Error("GenusCreateCommand is null");
        }
        var genus = new Genus
        {
            Name = request.Name,
            FamilyId = request.FamilyId
        };
        await genusRepository.AddAsync(genus, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Genus created successfully with ID: {GenusId}", genus.Id);
        return ServiceResult.SuccessAsCreated("/api/Genera/"+genus.Id);
    }
}
