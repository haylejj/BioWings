using BioWings.Application.Features.Commands.SpeciesTypeCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Write;
public class SpeciesTypeCreateCommandHandler(ISpeciesTypeRepository speciesTypeRepository, IUnitOfWork unitOfWork, ILogger<SpeciesTypeCreateCommandHandler> logger) : IRequestHandler<SpeciesTypeCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesTypeCreateCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("SpeciesTypeCreateCommand request is null");
            return ServiceResult.Error("SpeciesTypeCreateCommand request is null", HttpStatusCode.BadRequest);
        }
        var speciesType = new SpeciesType
        {
            Name = request.Name,
            Description = request.Description
        };
        await speciesTypeRepository.AddAsync(speciesType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("SpeciesType {Name} created", speciesType.Name);
        return ServiceResult.SuccessAsCreated("api/SpeciesTypes/" + speciesType.Id);
    }
}
