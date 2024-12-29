using BioWings.Application.Features.Results.ObservationMapResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationMapQueries;
public class ObservationMapGetByObservationIdQuery(int id):IRequest<ServiceResult<ObservationMapGetByObservationIdQueryResult>>
{
    public int ObservationId { get; set; } = id;
}
