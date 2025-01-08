using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Read;
public class GenusSearchQueryHandler(IGenusRepository genusRepository, ILogger<GenusSearchQueryHandler> logger) : IRequestHandler<GenusSearchQuery, ServiceResult<PaginatedList<GenusSearchQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<GenusSearchQueryResult>>> Handle(GenusSearchQuery request, CancellationToken cancellationToken)
    {
        var genus = genusRepository.GetAllAsQueryable().Include(g => g.Family).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            genus = genus.Where(g => g.Name.ToLower().Contains(searchTerm) || g.Family.Name.ToLower().Contains(searchTerm));
        }
        var items = await genus
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GenusSearchQueryResult
            {
                Id = x.Id,
                GenusName = x.Name,
                FamilyName = x.Family.Name,
                FamilyId = x.FamilyId
            }).ToListAsync(cancellationToken);
        var totalCount = items.Count;
        var paginatedResult = new PaginatedList<GenusSearchQueryResult>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Genus are filtered and fetched successfully.");
        return ServiceResult<PaginatedList<GenusSearchQueryResult>>.Success(paginatedResult);
    }
}
