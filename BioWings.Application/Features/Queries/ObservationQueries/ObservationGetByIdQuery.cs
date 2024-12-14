using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationQueries;
public class ObservationGetByIdQuery(int id) : IRequest<ServiceResult<ObservationGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
