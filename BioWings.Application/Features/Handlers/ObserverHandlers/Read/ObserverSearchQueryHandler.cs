using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObserverHandlers.Read;
public class ObserverSearchQueryHandler(IObserverRepository observerRepository, ILogger<ObserverSearchQueryHandler> logger) : IRequestHandler<ObserverSearchQuery, ServiceResult<PaginatedList<ObserverSearchQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObserverSearchQueryResult>>> Handle(ObserverSearchQuery request, CancellationToken cancellationToken)
    {
        var observers=observerRepository.GetAllAsNoTracking();
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            observers = observers.Where(s => s.Name.ToLower().Contains(searchTerm) || s.Surname.ToLower().Contains(searchTerm) || s.FullName.ToLower().Contains(searchTerm));
        }
        var totalCount = await observerRepository.GetTotalCountAsync(cancellationToken);
        var result = await observers.Select(x => new ObserverSearchQueryResult
        {
            Id = x.Id,
            FirstName = x.Name,
            LastName = x.Surname,
            FullName = x.FullName
        }).ToListAsync(cancellationToken);
        var paginatedResult = new PaginatedList<ObserverSearchQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Observers are filtered and fetched successfully.");
        return ServiceResult<PaginatedList<ObserverSearchQueryResult>>.Success(paginatedResult);
    }
}
