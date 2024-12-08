using BioWings.Application.Features.Commands.FamilyCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Write;
public class FamilyCreateCommandHandler(IFamilyRepository familyRepository, IUnitOfWork unitOfWork, ILogger<FamilyCreateCommandHandler> logger) : IRequestHandler<FamilyCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(FamilyCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("FamilyCreateCommand is null");
            return ServiceResult.Error("FamilyCreateCommand is null");
        }
        var family = new Family
        {
            Name = request.Name
        };
        await familyRepository.AddAsync(family);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Family created successfully with ID: {FamilyId}", family.Id);
        return ServiceResult.SuccessAsCreated("/api/Families/"+family.Id);
    }
}

