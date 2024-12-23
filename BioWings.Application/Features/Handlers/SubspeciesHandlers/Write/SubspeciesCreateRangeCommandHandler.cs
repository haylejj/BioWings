using BioWings.Application.Features.Commands.SubspeciesCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Write;
public class SubspeciesCreateRangeCommandHandler(ISubspeciesRepository subspeciesRepository, ILogger<SubspeciesCreateRangeCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<SubspeciesCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SubspeciesCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request == null || !request.Subspecies.Any())
        {
            logger.LogWarning("SubspeciesCreateRangeCommandHandler: request is null");
            return ServiceResult.Error("Request is null");
        }
        var subspecies = request.Subspecies.Select(s => new Subspecies
        {
            Name = s.Name,
            SpeciesId = s.SpeciesId
        });
        await subspeciesRepository.AddRangeAsync(subspecies, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Subspecies created");
        return ServiceResult.SuccessAsCreated("api/Subspecies/Range");
    }
}
