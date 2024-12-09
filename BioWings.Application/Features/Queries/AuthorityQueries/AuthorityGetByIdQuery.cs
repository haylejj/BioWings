using BioWings.Application.Features.Results.AuthorityResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.AuthorityQueries;
public class AuthorityGetByIdQuery(int id) : IRequest<ServiceResult<AuthorityGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
