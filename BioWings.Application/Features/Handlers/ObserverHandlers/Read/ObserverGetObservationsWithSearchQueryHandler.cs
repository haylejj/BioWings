using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObserverHandlers.Read;

public class ObserverGetObservationsWithSearchQueryHandler(IObserverRepository observerRepository, ILogger<ObserverGetObservationsWithSearchQueryHandler> logger) : IRequestHandler<ObserverGetObservationsWithSearchQuery, ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>> Handle(ObserverGetObservationsWithSearchQuery request, CancellationToken cancellationToken)
    {
        var query = observerRepository.GetAllAsNoTracking()
        .Where(x => x.Id == request.ObserverId)
        .SelectMany(x => x.Observations);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(o =>
                (o.Species.ScientificName != null && EF.Functions.Like(o.Species.ScientificName.ToLower(), $"%{searchTerm}%")) ||
                (o.Species.Name != null && EF.Functions.Like(o.Species.Name.ToLower(), $"%{searchTerm}%")) ||
                (o.Species.Genus.Name != null && EF.Functions.Like(o.Species.Genus.Name.ToLower(), $"%{searchTerm}%")) ||
                (o.Location.Province.Name != null && EF.Functions.Like(o.Location.Province.Name.ToLower(), $"%{searchTerm}%")) ||
                (o.Species.Genus.Family.Name != null && EF.Functions.Like(o.Species.Genus.Family.Name.ToLower(), $"%{searchTerm}%")) ||
                (o.Species.KocakName != null && EF.Functions.Like(o.Species.KocakName.ToLower(), $"%{searchTerm}%")) ||
                (o.Source != null && EF.Functions.Like(o.Source.ToLower(), $"%{searchTerm}%")) ||
                EF.Functions.Like(o.ObservationDate.ToString(), $"%{searchTerm}%")
            );
        }
        var queryWithSelect = query.Select(x => new ObserverGetObservationsQueryResult
        {
            Id = x.Id,
            ScientificName = x.Species.ScientificName,
            SpeciesName = x.Species.Name,
            KocakName = x.Species.KocakName,
            GenusName = x.Species.Genus.Name,
            FamilyName = x.Species.Genus.Family.Name,
            ProvinceName = x.Location.Province.Name,
            ObservationDate = x.ObservationDate,
            NumberSeen = x.NumberSeen,
            Source = x.Source
        });
        var totalCount = await queryWithSelect.CountAsync(cancellationToken);

        var items = await queryWithSelect
        .OrderByDescending(x => x.ObservationDate)
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync(cancellationToken);

        var paginatedResult = new PaginatedList<ObserverGetObservationsQueryResult>(
        items,
        totalCount,
        request.PageNumber,
        request.PageSize);

        logger.LogInformation("Filtered observations fetched successfully. Total count: {Count}", totalCount);
        return ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>.Success(paginatedResult);
    }
}
