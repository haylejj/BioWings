using BioWings.Application.Features.Queries.FamilyQueries;
using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.FamilyHandlers.Read;
public class FamilySearchQueryHandler(IFamilyRepository familyRepository, ILogger<FamilySearchQueryHandler> logger) : IRequestHandler<FamilySearchQuery, ServiceResult<PaginatedList<FamilySearchQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<FamilySearchQueryResult>>> Handle(FamilySearchQuery request, CancellationToken cancellationToken)
    {
        if (request.PageSize <= 0) request.PageSize = 25;
        if (request.PageNumber <= 0) request.PageNumber = 1;
        var families = familyRepository.GetAllAsQueryable().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            families = families.Where(f => f.Name.ToLower().Contains(searchTerm));
        }
        var items = await families
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new FamilySearchQueryResult
            {
                Id = x.Id,
                Name = x.Name,

            }).ToListAsync(cancellationToken);
        var totalCount = items.Count;
        var paginatedResult = new PaginatedList<FamilySearchQueryResult>(items, totalCount, request.PageNumber, request.PageSize);
        logger.LogInformation("Families are filtered and fetched successfully.");
        return ServiceResult<PaginatedList<FamilySearchQueryResult>>.Success(paginatedResult);
    }
}
