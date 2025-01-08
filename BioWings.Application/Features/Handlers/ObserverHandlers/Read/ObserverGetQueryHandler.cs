using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObserverHandlers.Read;

public class ObserverGetQueryHandler(IObserverRepository observerRepository,ILogger<ObserverGetQueryHandler> logger) : IRequestHandler<ObserverGetQuery, ServiceResult<List<ObserverGetQueryResult>>>
{
    public async Task<ServiceResult<List<ObserverGetQueryResult>>> Handle(ObserverGetQuery request, CancellationToken cancellationToken)
    {
        var observer=await observerRepository.GetAllAsQueryable().ToListAsync(cancellationToken);
        logger.LogInformation("Observers fetched successfully");
        return ServiceResult<List<ObserverGetQueryResult>>.Success(observer.Select(o => new ObserverGetQueryResult
        {
            Id=o.Id,
            FirstName=o.Name,
            LastName=o.Surname,
            FullName=o.FullName,
        }).ToList());
    }
}
