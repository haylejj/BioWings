using BioWings.Application.Features.Results.ProvinceResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ProvinceQueries;

public class ProvinceGetPagedQuery : IRequest<ServiceResult<PaginatedList<ProvinceGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
