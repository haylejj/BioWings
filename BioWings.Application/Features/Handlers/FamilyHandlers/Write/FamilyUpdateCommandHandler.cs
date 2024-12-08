using BioWings.Application.Features.Commands.FamilyCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Write;
public class FamilyUpdateCommandHandler(IFamilyRepository familyRepository, ILogger<FamilyUpdateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<FamilyUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(FamilyUpdateCommand request, CancellationToken cancellationToken)
    {
        if(request == null)
        {
            logger.LogWarning("FamilyUpdateCommand is null");
            return ServiceResult.Error("FamilyUpdateCommand is null");
        }
        var family = await familyRepository.GetByIdAsync(request.Id);
        if (family == null)
        {
            logger.LogWarning($"Family with id {request.Id} not found.");
            return ServiceResult.Error($"Family with id {request.Id} not found.",System.Net.HttpStatusCode.NotFound);
        }
        family.Name = request.Name;
        familyRepository.Update(family);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Family with id {request.Id} updated.");
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
