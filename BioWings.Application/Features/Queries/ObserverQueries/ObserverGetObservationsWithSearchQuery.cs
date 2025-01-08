using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObserverQueries;
public class ObserverGetObservationsWithSearchQuery : IRequest<ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>>
{
    public int ObserverId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
