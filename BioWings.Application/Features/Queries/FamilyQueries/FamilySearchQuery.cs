using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.FamilyQueries;
public class FamilySearchQuery : IRequest<ServiceResult<PaginatedList<FamilySearchQueryResult>>>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
