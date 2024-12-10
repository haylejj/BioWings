using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesQueries;
public class SpeciesGetQuery : IRequest<ServiceResult<IEnumerable<SpeciesGetQueryResult>>>
{
}
