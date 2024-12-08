using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesTypeQueries;
public class SpeciesTypeGetQuery : IRequest<ServiceResult<IEnumerable<SpeciesTypeGetQueryResult>>>
{
}
