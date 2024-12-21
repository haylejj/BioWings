using BioWings.Application.Features.Commands.FamilyCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Write;
public class FamilyCreateRangeCommandHandler(IFamilyRepository familyRepository, IUnitOfWork unitOfWork, ILogger<FamilyCreateRangeCommandHandler> logger) : IRequestHandler<FamilyCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(FamilyCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request.Families == null || !request.Families.Any())
        {
            logger.LogWarning("No families found to create");
            return ServiceResult.Error("No families found to create");
        }
        var families = request.Families.Select(f => new Family
        {
            Name = f.Name
        });
        await familyRepository.AddRangeAsync(families, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Families created successfully");
        return ServiceResult.SuccessAsCreated("/api/Families/Range");
    }
}
