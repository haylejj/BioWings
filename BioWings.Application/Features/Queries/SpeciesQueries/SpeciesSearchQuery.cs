using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesQueries;
public class SpeciesSearchQuery : IRequest<ServiceResult<PaginatedList<SpeciesSearchQueryResult>>>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
