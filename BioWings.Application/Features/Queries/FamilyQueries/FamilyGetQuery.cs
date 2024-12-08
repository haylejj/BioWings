using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.FamilyQueries;
public class FamilyGetQuery : IRequest<ServiceResult<IEnumerable<FamilyGetQueryResult>>>
{
}
