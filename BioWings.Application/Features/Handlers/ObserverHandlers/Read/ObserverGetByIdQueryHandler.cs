using BioWings.Application.Features.Queries.ObserverQueries;
using BioWings.Application.Features.Results.ObserverResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObserverHandlers.Read;

public class ObserverGetByIdQueryHandler(IObserverRepository observerRepository, ILogger<ObserverGetByIdQueryHandler> logger) : IRequestHandler<ObserverGetByIdQuery, ServiceResult<ObserverGetByIdQueryResult>>
{
    public async Task<ServiceResult<ObserverGetByIdQueryResult>> Handle(ObserverGetByIdQuery request, CancellationToken cancellationToken)
    {
        var observer=await  observerRepository.GetByIdAsync(request.Id);
        if (observer==null)
        {
            logger.LogError($"Observer with ID : {request.Id} was not found");
            return ServiceResult<ObserverGetByIdQueryResult>.Error($"Observer not found that has id : {request.Id}",System.Net.HttpStatusCode.NotFound);
        }
        var observerResult = new ObserverGetByIdQueryResult
        {
            Id = observer.Id,
            FirstName = observer.Name,
            LastName = observer.Surname,
            FullName = observer.FullName
        };
        logger.LogInformation("Retrieved observer {ObserverName} (ID: {ObserverId})",
            observer.FullName,
            observer.Id);
        return ServiceResult<ObserverGetByIdQueryResult>.Success(observerResult);

    }
}
