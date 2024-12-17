using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationSearchQueryHandler(IObservationRepository observationRepository, ILogger<ObservationSearchQueryHandler> logger) : IRequestHandler<ObservationSearchQuery, ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>> Handle(ObservationSearchQuery request, CancellationToken cancellationToken)
    {
        var observations = observationRepository.GetAllAsQueryable().Include(o => o.Species)
                .ThenInclude(s => s.Genus)
                    .ThenInclude(g => g.Family)
            .Include(o => o.Location)
                .ThenInclude(l => l.Province)
            .Include(o => o.Observer).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            observations = observations.Where(o =>
                o.Species.ScientificName.ToLower().Contains(searchTerm) ||
                o.Species.Genus.Name.ToLower().Contains(searchTerm) ||
                o.Species.Genus.Family.Name.ToLower().Contains(searchTerm) ||
                o.Species.HesselbarthName.ToLower().Contains(searchTerm) ||
                o.Location.Province.Name.ToLower().Contains(searchTerm) ||
                o.Species.FullName.ToLower().Contains(searchTerm)
            );
        }
        var totalCount = await observations.CountAsync(cancellationToken);
        var items = await observations
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            }).ToListAsync(cancellationToken);
        var paginatedResult = new PaginatedList<ObservationGetPagedQueryResult>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Observations are filtered and fetched successfully.");
        return ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>.Success(paginatedResult);

    }
}
