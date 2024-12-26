using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Read;
public class GenusGetPagedQueryHandler(IGenusRepository genusRepository, ILogger<GenusGetPagedQueryHandler> logger) : IRequestHandler<GenusGetPagedQuery, ServiceResult<PaginatedList<GenusGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<GenusGetPagedQueryResult>>> Handle(GenusGetPagedQuery request, CancellationToken cancellationToken)
    {
        // Validasyon
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await genusRepository.GetTotalCountAsync(cancellationToken);

        var genera = await genusRepository.GetPagedAsQueryable(request.PageNumber, request.PageSize)
                    .Include(x=> x.Family).AsNoTracking().ToListAsync(cancellationToken);

        var result = genera.Select(g => new GenusGetPagedQueryResult
        {
            Id = g.Id,
            GenusName = g.Name,
            FamilyId = g.FamilyId,
            FamilyName = g.Family?.Name
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

