using BioWings.Application.Features.Queries.FamilyQueries;
using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Read;
public class FamilyGetQueryHandler(IFamilyRepository familyRepository, ILogger<FamilyGetQueryHandler> logger) : IRequestHandler<FamilyGetQuery, ServiceResult<IEnumerable<FamilyGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<FamilyGetQueryResult>>> Handle(FamilyGetQuery request, CancellationToken cancellationToken)
    {
        var families = await familyRepository.GetAllAsync(cancellationToken);
        var result = families.Select(f => new FamilyGetQueryResult
        {
            Id = f.Id,
            Name = f.Name
        });
        logger.LogInformation("Families found successfully");
        return ServiceResult<IEnumerable<FamilyGetQueryResult>>.Success(result);
    }
}
