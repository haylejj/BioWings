using BioWings.Application.Features.Queries.SpeciesTypeQueries;
using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Read;
public class SpeciesTypeGetPagedQueryHandler(ISpeciesTypeRepository speciesTypeRepository, ILogger<SpeciesTypeGetPagedQueryHandler> logger) : IRequestHandler<SpeciesTypeGetPagedQuery, ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>> Handle(SpeciesTypeGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await speciesTypeRepository.GetTotalCountAsync();
        if (totalCount == 0)
        {
            logger.LogWarning("No species type found with paged");
            return ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>.Error("No species type found", HttpStatusCode.NotFound);
        }
        var speciesTypes = await speciesTypeRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (speciesTypes == null || !speciesTypes.Any())
        {
            logger.LogWarning("No species type found with paged");
            return ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>.Error("No species type found with paged", HttpStatusCode.NotFound);
        }
        var result = speciesTypes.Select(x => new SpeciesTypeGetPagedQueryResult
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        });
        var paginatedResult = new PaginatedList<SpeciesTypeGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Species types found successfully with paged");
        return ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>.Success(paginatedResult);
    }
}
