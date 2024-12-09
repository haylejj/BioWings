using BioWings.Application.Features.Results.AuthorityResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.AuthorityQueries;

public class AuthorityGetQuery : IRequest<ServiceResult<IEnumerable<AuthorityGetQueryResult>>>
{
}
