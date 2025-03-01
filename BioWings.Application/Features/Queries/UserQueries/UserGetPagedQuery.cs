using BioWings.Application.Features.Results.UserResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.UserQueries;
public class UserGetPagedQuery : IRequest<ServiceResult<PaginatedList<UserGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }  // kullanıcı girecek
    public int PageSize { get; set; }    // kullanıcı girecek
}
