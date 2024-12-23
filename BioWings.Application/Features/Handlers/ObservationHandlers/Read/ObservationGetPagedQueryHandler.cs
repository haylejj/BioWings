using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationGetPagedQueryHandler(IObservationRepository observationRepository, ILogger<ObservationGetPagedQueryHandler> logger) : IRequestHandler<ObservationGetPagedQuery, ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>> Handle(ObservationGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await observationRepository.GetTotalCountAsync(cancellationToken);

        var observations = await observationRepository.GetPagedAsQueryable(request.PageNumber, request.PageSize).Include(x => x.Observer)
            .Include(x => x.Location).ThenInclude(y => y.Province)
            .Include(x => x.Species).ThenInclude(y => y.Genus).ThenInclude(z => z.Family)
            .Select(x => new ObservationGetPagedQueryResult
            {
                Id = x.Id,
                // Species related
                AuthorityName = x.Species.Authority.Name,
                Year = x.Species.Authority.Year,
                GenusName = x.Species.Genus.Name,
                FamilyName = x.Species.Genus.Family.Name,
                ScientificName = x.Species.ScientificName,
                Name = x.Species.Name,
                EUName = x.Species.EUName,
                FullName = x.Species.FullName,
                HesselbarthName = x.Species.HesselbarthName,
                TurkishName = x.Species.TurkishName,
                EnglishName = x.Species.EnglishName,
                TurkishNamesTrakel = x.Species.TurkishNamesTrakel,
                Trakel = x.Species.Trakel,
                KocakName = x.Species.KocakName,

                // Location related
                ProvinceName = x.Location.Province.Name,
                SquareRef = x.Location.SquareRef,
                SquareLatitude = x.Location.SquareLatitude,
                SquareLongitude = x.Location.SquareLongitude,
                Latitude = x.Location.Latitude,
                Longitude = x.Location.Longitude,
                DecimalDegrees = x.Location.DecimalDegrees,
                DegreesMinutesSeconds = x.Location.DegreesMinutesSeconds,
                DecimalMinutes = x.Location.DecimalMinutes,
                UtmCoordinates = x.Location.UtmCoordinates,
                MgrsCoordinates = x.Location.MgrsCoordinates,
                Altitude1 = x.Location.Altitude1,
                Altitude2 = x.Location.Altitude2,
                UtmReference = x.Location.UtmReference,
                CoordinatePrecisionLevel = x.Location.CoordinatePrecisionLevel,

                // Observer and other fields
                ObserverFullName = x.Observer.FullName,
                Sex = x.Sex,
                ObservationDate = x.ObservationDate,
                LifeStage = x.LifeStage,
                NumberSeen = x.NumberSeen,
                Notes = x.Notes,
                Source = x.Source,
                LocationInfo = x.LocationInfo
            })
            .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedList<ObservationGetPagedQueryResult>(
            observations,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Observations are fetched successfully.");
        return ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>.Success(paginatedResult);

    }
}
