using BioWings.Application.Features.Results.SubspeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SubspeciesQueries;
public class SubspeciesGetPagedQuery: IRequest<ServiceResult<PaginatedList<SubspeciesGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }  // kullanıcı girecek
    public int PageSize { get; set; }    // kullanıcı girecek
}
