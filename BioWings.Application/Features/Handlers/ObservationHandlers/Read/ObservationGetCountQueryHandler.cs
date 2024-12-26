using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationGetCountQueryHandler(ILogger<ObservationGetCountQueryHandler> logger, IObservationRepository observationRepository) : IRequestHandler<ObservationGetCountQuery, ServiceResult<ObservationGetCountQueryResult>>
{
    public async Task<ServiceResult<ObservationGetCountQueryResult>> Handle(ObservationGetCountQuery request, CancellationToken cancellationToken)
    {
        var count = await observationRepository.GetCountAsync(cancellationToken);
        logger.LogInformation("Observation count is {count}", count);
        return ServiceResult<ObservationGetCountQueryResult>.Success(new ObservationGetCountQueryResult { Count = count });
    }
}
