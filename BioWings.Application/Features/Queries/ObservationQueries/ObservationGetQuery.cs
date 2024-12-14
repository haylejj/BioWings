using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationQueries;
public class ObservationGetQuery : IRequest<ServiceResult<IEnumerable<ObservationGetQueryResult>>>
{
}
