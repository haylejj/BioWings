using BioWings.Application.Features.Results.LocationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.LocationQueries;
public class LocationGetQuery:IRequest<ServiceResult<IEnumerable<LocationGetQueryResult>>>
{
}
