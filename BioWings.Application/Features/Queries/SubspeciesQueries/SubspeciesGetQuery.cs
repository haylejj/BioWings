using BioWings.Application.Features.Results.SubspeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SubspeciesQueries;
public class SubspeciesGetQuery:IRequest<ServiceResult<IEnumerable<SubspeciesGetQueryResult>>>
{
}
