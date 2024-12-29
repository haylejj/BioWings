using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ExportQueries;
public class ExportObservationsByProvinceQuery(int provinceId) : IRequest<ServiceResult<byte[]>>
{
    public int ProvinceId { get; set; } = provinceId;
}
