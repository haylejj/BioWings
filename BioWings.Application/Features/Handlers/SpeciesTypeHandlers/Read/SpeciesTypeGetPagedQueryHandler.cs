using BioWings.Application.Features.Queries.SpeciesTypeQueries;
using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Read;
public class SpeciesTypeGetPagedQueryHandler(ISpeciesTypeRepository speciesTypeRepository, ILogger<SpeciesTypeGetPagedQueryHandler> logger) : IRequestHandler<SpeciesTypeGetPagedQuery, ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>> Handle(SpeciesTypeGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await speciesTypeRepository.GetTotalCountAsync(cancellationToken);

        var speciesTypes = await speciesTypeRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

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
