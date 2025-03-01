using BioWings.Application.Features.Results.CountryResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.CountryQueries;
public class CountryGetQuery : IRequest<ServiceResult<List<CountryGetQueryResult>>>
{
}
