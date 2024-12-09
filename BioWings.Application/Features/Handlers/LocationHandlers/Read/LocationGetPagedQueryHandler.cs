using BioWings.Application.Features.Queries.LocationQueries;
using BioWings.Application.Features.Results.LocationResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.LocationHandlers.Read;
public class LocationGetPagedQueryHandler(ILocationRepository locationRepository, ILogger<LocationGetPagedQueryHandler> logger) : IRequestHandler<LocationGetPagedQuery, ServiceResult<PaginatedList<LocationGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<LocationGetPagedQueryResult>>> Handle(LocationGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await locationRepository.GetTotalCountAsync(cancellationToken);

        var locations = await locationRepository.GetPagedAsQueryable(request.PageNumber, request.PageSize).Include(x => x.Province).ToListAsync(cancellationToken);

        var result = locations.Select(l => new LocationGetPagedQueryResult
        {
            Id = l.Id,
            ProvinceId = l.ProvinceId,
            ProvinceName = l.Province.Name,
            Name = l.Name,
            SquareRef = l.SquareRef,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Altitude1 = l.Altitude1,
            Altitude2 = l.Altitude2,
            XCoord = l.XCoord,
            YCoord = l.YCoord,
            UtmReference = l.UtmReference,
            Description = l.Description
        });
        var paginatedResult = new PaginatedList<LocationGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Locations found successfully with paged");
        return ServiceResult<PaginatedList<LocationGetPagedQueryResult>>.Success(paginatedResult);
    }
}
