using BioWings.Application.Features.Queries.FamilyQueries;
using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Read;
public class FamilyGetPagedQueryHandler(IFamilyRepository familyRepository, ILogger<FamilyGetPagedQueryHandler> logger) : IRequestHandler<FamilyGetPagedQuery, ServiceResult<PaginatedList<FamilyGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<FamilyGetPagedQueryResult>>> Handle(FamilyGetPagedQuery request, CancellationToken cancellationToken)
    {
        // Validasyon
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await familyRepository.GetTotalCountAsync(cancellationToken);

        var families = await familyRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var result = families.Select(f => new FamilyGetPagedQueryResult
        {
            Id = f.Id,
            Name = f.Name
        });
        var paginatedResult = new PaginatedList<FamilyGetPagedQueryResult>(
           result,
           totalCount,
           request.PageNumber,
           request.PageSize);
        logger.LogInformation("Families found successfully with paged");
        return ServiceResult<PaginatedList<FamilyGetPagedQueryResult>>.Success(paginatedResult);
    }
}
