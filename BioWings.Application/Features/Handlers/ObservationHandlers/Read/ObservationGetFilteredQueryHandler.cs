using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationGetFilteredQueryHandler(IObservationRepository observationRepository, ILogger<ObservationGetFilteredQueryHandler> logger) : IRequestHandler<ObservationGetFilteredQuery, ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>> Handle(ObservationGetFilteredQuery request, CancellationToken cancellationToken)
    {
        var (observations, totalCount) = await observationRepository.GetFilteredAsync(request.ColumnNames, request.ColumnValues, request.PageNumber, request.PageSize, cancellationToken);

        var observationViewModels = observations.Select(x => new ObservationGetPagedQueryResult
        {
            Id = x.Id,
            // Species related
            AuthorityName = x.Species?.Authority?.Name,
            Year = x.Species?.Authority?.Year,
            GenusName = x.Species?.Genus?.Name,
            FamilyName = x.Species?.Genus?.Family?.Name,
            ScientificName = x.Species?.ScientificName,
            Name = x.Species?.Name,
            EUName = x.Species?.EUName,
            FullName = x.Species?.FullName,
            HesselbarthName = x.Species?.HesselbarthName,
            TurkishName = x.Species?.TurkishName,
            EnglishName = x.Species?.EnglishName,
            TurkishNamesTrakel = x.Species?.TurkishNamesTrakel,
            Trakel = x.Species?.Trakel,
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
            ObserverFullName = x.Observer?.FullName,
            Sex = x.Sex,
            ObservationDate = x.ObservationDate,
            LifeStage = x.LifeStage,
            NumberSeen = x.NumberSeen,
            Notes = x.Notes,
            Source = x.Source,
            LocationInfo = x.LocationInfo

        }).ToList();

        var paginatedResult = new PaginatedList<ObservationGetPagedQueryResult>(
            observationViewModels,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Filtred Observations are fetched successfully.");
        return ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>.Success(paginatedResult);
    }
}
