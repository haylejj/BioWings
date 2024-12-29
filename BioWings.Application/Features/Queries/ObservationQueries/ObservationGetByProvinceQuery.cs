using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationQueries;
public class ObservationGetByProvinceQuery(int code, int pageNumber, int pageSize) : IRequest<ServiceResult<PaginatedList<ObservationGetByProvinceQueryResult>>>
{
    public int ProvinceCode { get; set; } = code;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}
