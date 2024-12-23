using BioWings.Application.Features.Queries.ProvinceQueries;
using BioWings.Application.Features.Results.ProvinceResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Read;
public class ProvinceGetByIdQueryHandler(IProvinceRepository provinceRepository, ILogger<ProvinceGetByIdQueryHandler> logger) : IRequestHandler<ProvinceGetByIdQuery, ServiceResult<ProvinceGetByIdQueryResult>>
{
    public async Task<ServiceResult<ProvinceGetByIdQueryResult>> Handle(ProvinceGetByIdQuery request, CancellationToken cancellationToken)
    {
        var province = await provinceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (province==null)
        {
            logger.LogError("Province Id:{0} not found", request.Id);
            return ServiceResult<ProvinceGetByIdQueryResult>.Error($"Province Id:{request.Id} not found", System.Net.HttpStatusCode.NotFound);
        }
        logger.LogInformation("Province Id:{0} found", request.Id);
        return ServiceResult<ProvinceGetByIdQueryResult>.Success(new ProvinceGetByIdQueryResult
        {
            Id=province.Id,
            Name=province.Name,
            ProvinceCode=province.ProvinceCode
        });
    }
}
