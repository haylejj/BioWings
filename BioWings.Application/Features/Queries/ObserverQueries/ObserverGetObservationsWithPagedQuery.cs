using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObserverQueries;
public class ObserverGetObservationsWithPagedQuery : IRequest<ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>>
{
    public int ObserverId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
