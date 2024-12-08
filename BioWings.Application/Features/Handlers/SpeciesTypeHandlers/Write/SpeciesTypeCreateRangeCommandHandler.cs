using BioWings.Application.Features.Commands.SpeciesTypeCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Write;

public class SpeciesTypeCreateRangeCommandHandler(ISpeciesTypeRepository speciesTypeRepository, IUnitOfWork unitOfWork, ILogger<SpeciesTypeCreateRangeCommandHandler> logger) : IRequestHandler<SpeciesTypeCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesTypeCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request is null || !request.SpeciesTypes.Any())
        {
            logger.LogWarning("SpeciesTypeCreateRangeCommand request is null or empty");
            return ServiceResult.Error("SpeciesTypeCreateRangeCommand request is null or empty", HttpStatusCode.BadRequest);
        }
        var speciesTypes = request.SpeciesTypes.Select(st => new SpeciesType
        {
            Name = st.Name,
            Description = st.Description
        });
        await speciesTypeRepository.AddRangeAsync(speciesTypes, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("SpeciesTypes created");
        return ServiceResult.SuccessAsCreated("api/SpeciesTypes/Range");
    }
}
