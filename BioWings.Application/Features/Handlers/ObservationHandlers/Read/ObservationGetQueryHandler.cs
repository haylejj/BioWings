using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationGetQueryHandler(IObservationRepository observationRepository, ILogger<ObservationGetQueryHandler> logger) : IRequestHandler<ObservationGetQuery, ServiceResult<IEnumerable<ObservationGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<ObservationGetQueryResult>>> Handle(ObservationGetQuery request, CancellationToken cancellationToken)
    {
        var observations = await observationRepository.GetAllAsQueryable().Include(x => x.Observer)
            .Include(x => x.Location).ThenInclude(y => y.Province)
            .Include(x => x.Species).ThenInclude(y => y.Genus).ThenInclude(z => z.Family)
            .Select(x => new ObservationGetQueryResult
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
        logger.LogInformation("Observations are fetched successfully.");
        return ServiceResult<IEnumerable<ObservationGetQueryResult>>.Success(observations);


    }
}
