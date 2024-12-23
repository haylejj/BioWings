using BioWings.Application.Features.Queries.ProvinceQueries;
using BioWings.Application.Features.Results.ProvinceResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Read;
public class ProvinceGetPagedQueryHandler(IProvinceRepository provinceRepository, ILogger<ProvinceGetPagedQueryHandler> logger) : IRequestHandler<ProvinceGetPagedQuery, ServiceResult<PaginatedList<ProvinceGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ProvinceGetPagedQueryResult>>> Handle(ProvinceGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await provinceRepository.GetTotalCountAsync(cancellationToken);

        var provinces = await provinceRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var result = provinces.Select(p => new ProvinceGetPagedQueryResult
        {
            Id = p.Id,
            Name = p.Name,
            ProvinceCode = p.ProvinceCode
        });
        var paginatedResult = new PaginatedList<ProvinceGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Provinces found successfully with paged");
        return ServiceResult<PaginatedList<ProvinceGetPagedQueryResult>>.Success(paginatedResult);
    }
}
