using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationQueries;
public class ObservationGetPagedQuery : IRequest<ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

}
