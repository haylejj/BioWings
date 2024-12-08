using BioWings.Application.Features.Queries.FamilyQueries;
using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Read;
public class FamilyGetByIdQueryHandler(IFamilyRepository familyRepository, ILogger<FamilyGetByIdQueryHandler> logger) : IRequestHandler<FamilyGetByIdQuery, ServiceResult<FamilyGetByIdQueryResult>>
{
    public async Task<ServiceResult<FamilyGetByIdQueryResult>> Handle(FamilyGetByIdQuery request, CancellationToken cancellationToken)
    {
        var family = await familyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (family == null)
        {
            logger.LogWarning("Family not found with ID: {FamilyId}", request.Id);
            return ServiceResult<FamilyGetByIdQueryResult>.Error("Family not found", HttpStatusCode.NotFound);
        }
        var result = new FamilyGetByIdQueryResult
        {
            Id = family.Id,
            Name = family.Name
        };
        logger.LogInformation("Family found with ID: {FamilyId}", family.Id);
        return ServiceResult<FamilyGetByIdQueryResult>.Success(result);
    }
}
