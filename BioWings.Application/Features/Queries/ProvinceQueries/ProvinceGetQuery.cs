using BioWings.Application.Features.Results.ProvinceResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ProvinceQueries;
public class ProvinceGetQuery : IRequest<ServiceResult<IEnumerable<ProvinceGetQueryResult>>>
{
}
