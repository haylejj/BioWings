using BioWings.Application.Features.Queries.ObservationMapQueries;
using BioWings.Application.Features.Results.ObservationMapResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationMapHandlers;
public class ObservationMapGetQueryHandler(IObservationRepository observationRepository, ILogger<ObservationMapGetQueryHandler> logger) : IRequestHandler<ObservationMapGetQuery, ServiceResult<List<ObservationMapGetQueryResult>>>
{
    public async Task<ServiceResult<List<ObservationMapGetQueryResult>>> Handle(ObservationMapGetQuery request, CancellationToken cancellationToken)

    {
        var observations = observationRepository.GetAllAsQueryable()
           .Include(x => x.Location)
           .ThenInclude(x => x.Province)
           .AsNoTracking();

        // Zoom seviyesine göre nokta sayısını sınırla
        int maxPoints = request.ZoomLevel switch
        {
            <= 8 => 5000,  // Bölge seviyesi
            <= 10 => 10000, // İl seviyesi
            _ => 15000      // Detaylı görünüm
        };

        var result = await observations
            .Where(x => x.Location.Latitude >= Convert.ToDecimal(request.MinLat) &&
                       x.Location.Latitude <= Convert.ToDecimal(request.MaxLat) &&
                       x.Location.Longitude >= Convert.ToDecimal(request.MinLng) &&
                       x.Location.Longitude <= Convert.ToDecimal(request.MaxLng))
            .OrderBy(x => Guid.NewGuid()) // Rastgele sırala
            .Take(maxPoints)
            .Select(x => new ObservationMapGetQueryResult
            {
                Id = x.Id,
                Latitude = x.Location.Latitude,
                Longitude = x.Location.Longitude,
                ProvinceCode = x.Location.Province.ProvinceCode,
                ProvinceName = x.Location.Province.Name
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation($"Retrieved {result.Count} observation points for zoom level {request.ZoomLevel}");
        return ServiceResult<List<ObservationMapGetQueryResult>>.Success(result);
    }
}
