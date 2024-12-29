using BioWings.Application.Features.Queries.ObservationMapQueries;
using BioWings.Application.Features.Results.ObservationMapResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationMapHandlers;
public class ObservationMapGetByObservationIdQueryHandler(IObservationRepository observationRepository, ILogger<ObservationMapGetByObservationIdQueryHandler> logger) : IRequestHandler<ObservationMapGetByObservationIdQuery, ServiceResult<ObservationMapGetByObservationIdQueryResult>>
{
    public async Task<ServiceResult<ObservationMapGetByObservationIdQueryResult>> Handle(ObservationMapGetByObservationIdQuery request, CancellationToken cancellationToken)
    {
        var observation = await observationRepository.GetByIdWithAllNavigationsAsync(request.ObservationId);
        if (observation==null)
        {
            logger.LogError($"Observation that has id : {request.ObservationId} not found");
            return ServiceResult<ObservationMapGetByObservationIdQueryResult>.Error($"Observation that has id : {request.ObservationId} not found", System.Net.HttpStatusCode.NotFound);
        }
        var observationMap = new ObservationMapGetByObservationIdQueryResult
        {
            Id=observation.Id,
            Latitude=observation.Location.Latitude,
            Longitude=observation.Location.Longitude,
            ProvinceName=observation.Location?.Province.Name,
            AuthorityYear=observation.Species.Authority?.Year,
            AuthortyName=observation.Species.Authority?.Name,
            FamilyName=observation.Species.Genus?.Family?.Name,
            GenusName=observation.Species.Genus?.Name,
            HesselbartName=observation.Species.HesselbarthName,
            KocakName=observation.Species.KocakName,
            ScientificName=observation.Species.ScientificName,
            SpeciesName=observation.Species.Name,
            Notes=observation.Notes,
            NumberSeen=observation.NumberSeen,
            ObservationDate=observation.ObservationDate
        };
        logger.LogInformation($"Observation that has id : {request.ObservationId} found.");
        return ServiceResult<ObservationMapGetByObservationIdQueryResult>.Success(observationMap);
    }
}
