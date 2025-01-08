using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObserverQueries;
public class ObserverGetByIdQuery(int id): IRequest<ServiceResult<ObserverGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
