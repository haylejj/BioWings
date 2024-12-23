using BioWings.Application.Features.Commands.SubspeciesCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Write;
public class SubspeciesCreateCommandHandler(ISubspeciesRepository subspeciesRepository, ILogger<SubspeciesCreateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<SubspeciesCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SubspeciesCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("SubspeciesCreateCommandHandler: request is null");
            return ServiceResult.Error("Request is null");
        }
        var subspecies = new Subspecies
        {
            Name=request.Name,
            SpeciesId=request.SpeciesId
        };
        await subspeciesRepository.AddAsync(subspecies, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Subspecies Id : {0} created successfully", subspecies.Id);
        return ServiceResult.SuccessAsCreated("api/Subspecies/"+subspecies.Id);

    }
}
