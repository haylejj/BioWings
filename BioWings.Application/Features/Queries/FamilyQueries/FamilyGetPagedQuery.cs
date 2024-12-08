using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.FamilyQueries;
public class FamilyGetPagedQuery:IRequest<ServiceResult<PaginatedList<FamilyGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }  // kullanıcı girecek
    public int PageSize { get; set; }    // kullanıcı girecek
}
