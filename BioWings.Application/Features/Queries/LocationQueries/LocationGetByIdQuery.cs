using BioWings.Application.Features.Results.LocationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.LocationQueries;

public class LocationGetByIdQuery(int id) : IRequest<ServiceResult<LocationGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
