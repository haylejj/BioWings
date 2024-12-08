using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Read;
public class GenusGetPagedQueryHandler(IGenusRepository genusRepository, ILogger<GenusGetPagedQueryHandler> logger) : IRequestHandler<GenusGetPagedQuery, ServiceResult<PaginatedList<GenusGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<GenusGetPagedQueryResult>>> Handle(GenusGetPagedQuery request, CancellationToken cancellationToken)
    {
        // Validasyon
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await genusRepository.GetTotalCountAsync();
        if (totalCount==0)
        {
            logger.LogWarning("No genus found with paged");
            return ServiceResult<PaginatedList<GenusGetPagedQueryResult>>.Error("No genera found", HttpStatusCode.NotFound);
        }
        var genera = await genusRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        if (genera == null || !genera.Any())
        {
            logger.LogWarning("No genus found with paged");
            return ServiceResult<PaginatedList<GenusGetPagedQueryResult>>.Error("No genus found with paged", HttpStatusCode.NotFound);
        }
        var result = genera.Select(g => new GenusGetPagedQueryResult
        {
            Id = g.Id,
            Name = g.Name
        });
        var paginatedResult = new PaginatedList<GenusGetPagedQueryResult>(
           result,
           totalCount,
           request.PageNumber,
           request.PageSize);
        logger.LogInformation("Genera found successfully with paged");
        return ServiceResult<PaginatedList<GenusGetPagedQueryResult>>.Success(paginatedResult);
    }
}

