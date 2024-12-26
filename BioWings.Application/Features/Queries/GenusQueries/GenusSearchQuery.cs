using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.GenusQueries;
public class GenusSearchQuery : IRequest<ServiceResult<PaginatedList<GenusSearchQueryResult>>>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
