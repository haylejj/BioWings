using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationGetByIdQueryHandler(IObservationRepository observationRepository, ILogger<ObservationGetByIdQueryHandler> logger) : IRequestHandler<ObservationGetByIdQuery, ServiceResult<ObservationGetByIdQueryResult>>
{
    public async Task<ServiceResult<ObservationGetByIdQueryResult>> Handle(ObservationGetByIdQuery request, CancellationToken cancellationToken)
    {
        var observation = await observationRepository.GetByIdWithAllNavigationsAsync(request.Id, cancellationToken);
        return ServiceResult<ObservationGetByIdQueryResult>.Success(new ObservationGetByIdQueryResult
        {
            Id = observation.Id,
            // Species related
            AuthorityName = observation.Species?.Authority?.Name,
            Year = observation.Species?.Authority?.Year,
            GenusName = observation.Species?.Genus?.Name,
            FamilyName = observation.Species?.Genus?.Family?.Name,
            ScientificName = observation.Species?.ScientificName,
            Name = observation.Species?.Name,
            EUName = observation.Species?.EUName,
            FullName = observation.Species?.FullName,
            HesselbarthName = observation.Species?.HesselbarthName,
            TurkishName = observation.Species?.TurkishName,
            EnglishName = observation.Species?.EnglishName,
            TurkishNamesTrakel = observation.Species?.TurkishNamesTrakel,
            Trakel = observation.Species?.Trakel,
            KocakName = observation.Species?.KocakName,

            // Location related
            ProvinceId = observation.Location?.Province?.Id ?? 0,
            ProvinceName = observation.Location?.Province?.Name,
            SquareRef = observation.Location?.SquareRef,
            SquareLatitude = observation.Location?.SquareLatitude ?? 0,
            SquareLongitude = observation.Location?.SquareLongitude ?? 0,
            Latitude = observation.Location?.Latitude ?? 0,
            Longitude = observation.Location?.Longitude ?? 0,
            DecimalDegrees = observation.Location?.DecimalDegrees,
            DegreesMinutesSeconds = observation.Location?.DegreesMinutesSeconds,
            DecimalMinutes = observation.Location?.DecimalMinutes,
            UtmCoordinates = observation.Location?.UtmCoordinates,
            MgrsCoordinates = observation.Location?.MgrsCoordinates,
            Altitude1 = observation.Location?.Altitude1 ?? 0,
            Altitude2 = observation.Location?.Altitude2 ?? 0,
            UtmReference = observation.Location?.UtmReference,
            CoordinatePrecisionLevel = observation.Location?.CoordinatePrecisionLevel ?? 0,

            // Observer and other fields
            ObserverFullName = observation.Observer?.FullName,
            Sex = observation.Sex,
            ObservationDate = observation.ObservationDate,
            LifeStage = observation.LifeStage,
            NumberSeen = observation.NumberSeen,
            Notes = observation.Notes,
            Source = observation.Source,
            LocationInfo = observation.LocationInfo
        });
    }
}
