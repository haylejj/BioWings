using BioWings.Application.Features.Queries.SubspeciesQueries;
using BioWings.Application.Features.Results.SubspeciesResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Read;

public class SubspeciesGetPagedQueryHandler(ISubspeciesRepository subspeciesRepository, ILogger<SubspeciesGetPagedQueryHandler> logger) : IRequestHandler<SubspeciesGetPagedQuery, ServiceResult<PaginatedList<SubspeciesGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<SubspeciesGetPagedQueryResult>>> Handle(SubspeciesGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await subspeciesRepository.GetTotalCountAsync(cancellationToken);

        var subspecies = await subspeciesRepository.GetPagedAsQueryable(request.PageNumber, request.PageSize).Include(x => x.Species).ToListAsync();

        var result = subspecies.Select(x => new SubspeciesGetPagedQueryResult
        {
            Id = x.Id,
            Name = x.Name,
            SpeciesId = x.SpeciesId,
            SpeciesName = x.Species.Name
        });
        var paginatedResult = new PaginatedList<SubspeciesGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Subspecies found successfully with paged");
        return ServiceResult<PaginatedList<SubspeciesGetPagedQueryResult>>.Success(paginatedResult);
    }
}
