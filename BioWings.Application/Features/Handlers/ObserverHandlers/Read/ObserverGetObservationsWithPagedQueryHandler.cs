using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObserverHandlers.Read;
public class ObserverGetObservationsWithPagedQueryHandler(IObserverRepository observerRepository, ILogger<ObserverGetObservationsWithPagedQueryHandler> logger) : IRequestHandler<ObserverGetObservationsWithPagedQuery, ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>> Handle(ObserverGetObservationsWithPagedQuery request, CancellationToken cancellationToken)
    {
        var query = observerRepository.GetAllAsNoTracking()
        .Where(x => x.Id == request.ObserverId)
        .SelectMany(x => x.Observations)
        .Select(x => new ObserverGetObservationsQueryResult
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
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.ObservationDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        var paginatedResult = new PaginatedList<ObserverGetObservationsQueryResult>(
           items,
           totalCount,
           request.PageNumber,
           request.PageSize);
        logger.LogInformation("Observations are fetched successfully.Returning {Count} Observations", totalCount);
        return ServiceResult<PaginatedList<ObserverGetObservationsQueryResult>>.Success(paginatedResult);
    }
}
