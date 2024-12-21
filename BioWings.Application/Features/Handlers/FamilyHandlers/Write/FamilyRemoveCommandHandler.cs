using BioWings.Application.Features.Commands.FamilyCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Write;
public class FamilyRemoveCommandHandler(IFamilyRepository familyRepository, ILogger<FamilyRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<FamilyRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(FamilyRemoveCommand request, CancellationToken cancellationToken)
    {
        var family = await familyRepository.GetByIdAsync(request.Id);
        if (family == null)
        {
            logger.LogWarning("Family not found with ID: {FamilyId}", request.Id);
            return ServiceResult.Error("Family not found", HttpStatusCode.NotFound);
        }
        familyRepository.Remove(family);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Family removed successfully with ID: {FamilyId}", family.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
