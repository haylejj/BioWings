using BioWings.Application.Features.Queries.ProvinceQueries;
using BioWings.Application.Features.Results.ProvinceResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Read;
public class ProvinceGetQueryHandler(IProvinceRepository provinceRepository, ILogger<ProvinceGetQueryHandler> logger) : IRequestHandler<ProvinceGetQuery, ServiceResult<IEnumerable<ProvinceGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<ProvinceGetQueryResult>>> Handle(ProvinceGetQuery request, CancellationToken cancellationToken)
    {
        var provinces = await provinceRepository.GetAllAsync(cancellationToken);
        logger.LogInformation("Provinces found");
        return ServiceResult<IEnumerable<ProvinceGetQueryResult>>.Success(provinces.Select(p => new ProvinceGetQueryResult
        {
            Id=p.Id,
            Name=p.Name,
            ProvinceCode=p.ProvinceCode
        }));
    }
}


