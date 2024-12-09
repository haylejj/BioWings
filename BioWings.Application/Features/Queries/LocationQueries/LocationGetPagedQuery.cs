using BioWings.Application.Features.Results.LocationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.LocationQueries;

public class LocationGetPagedQuery : IRequest<ServiceResult<PaginatedList<LocationGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
