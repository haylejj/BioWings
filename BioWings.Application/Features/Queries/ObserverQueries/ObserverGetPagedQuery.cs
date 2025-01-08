using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObserverQueries;
public class ObserverGetPagedQuery : IRequest<ServiceResult<PaginatedList<ObserverGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
