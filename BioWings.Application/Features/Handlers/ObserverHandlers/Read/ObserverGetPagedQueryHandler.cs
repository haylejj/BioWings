using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObserverHandlers.Read;
public class ObserverGetPagedQueryHandler(IObserverRepository observerRepository, ILogger<ObserverGetPagedQueryHandler> logger) : IRequestHandler<ObserverGetPagedQuery, ServiceResult<PaginatedList<ObserverGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObserverGetPagedQueryResult>>> Handle(ObserverGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await observerRepository.GetTotalCountAsync(cancellationToken);

        var observer = await observerRepository.GetPagedAsNoTrackingAsync(request.PageNumber, request.PageSize, cancellationToken);
        var result = observer.Select(x => new ObserverGetPagedQueryResult
        {
            Id = x.Id,
            FirstName = x.Name,
            LastName = x.Surname,
            FullName = x.FullName
        });
        var paginatedResult = new PaginatedList<ObserverGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Observers are fetched successfully.Returning {Count} Observers", observer.Count());
        return ServiceResult<PaginatedList<ObserverGetPagedQueryResult>>.Success(paginatedResult);
    }
}
