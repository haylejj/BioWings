using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObserverQueries;
public class ObserverGetQuery:IRequest<ServiceResult<List<ObserverGetQueryResult>>>
{
}
