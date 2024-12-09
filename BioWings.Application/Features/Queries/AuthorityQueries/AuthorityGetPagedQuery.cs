using BioWings.Application.Features.Results.AuthorityResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.AuthorityQueries;
public class AuthorityGetPagedQuery : IRequest<ServiceResult<PaginatedList<AuthorityGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
