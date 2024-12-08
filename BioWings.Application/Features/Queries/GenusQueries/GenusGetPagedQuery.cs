using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.GenusQueries;
public class GenusGetPagedQuery:IRequest<ServiceResult<PaginatedList<GenusGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }  // kullanıcı girecek
    public int PageSize { get; set; }    // kullanıcı girecek
}
