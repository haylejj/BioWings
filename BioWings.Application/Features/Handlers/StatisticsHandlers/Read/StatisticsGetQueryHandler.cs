using BioWings.Application.Features.Queries.StatisticsQueries;
using BioWings.Application.Features.Results.StatisticResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.StatisticsHandlers.Read;
public class StatisticsGetQueryHandler(ILogger<StatisticsGetQueryHandler> logger, IObservationRepository observationRepository, ISpeciesRepository speciesRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository, IObserverRepository observerRepository) : IRequestHandler<StatisticsGetQuery, ServiceResult<StatisticsGetQueryResult>>
{
    public async Task<ServiceResult<StatisticsGetQueryResult>> Handle(StatisticsGetQuery request, CancellationToken cancellationToken)
    {
        var statistics = new StatisticsGetQueryResult();
        statistics.TotalObservationCount= await observationRepository.GetTotalCountAsync(cancellationToken);
        statistics.TotalSpeciesCount= await speciesRepository.GetTotalCountAsync(cancellationToken);
        statistics.TotalFamilyCount= await familyRepository.GetTotalCountAsync(cancellationToken);
        statistics.TotalGenusCount= await genusRepository.GetTotalCountAsync(cancellationToken);
        statistics.TotalObserverCount= await observerRepository.GetTotalCountAsync(cancellationToken);

        statistics.TopProvinces= await observationRepository.GetTopProvincesAsync(cancellationToken);
        statistics.TopFamilies= await familyRepository.GetTopFamiliesAsync(cancellationToken);
        statistics.TopSpecies= await speciesRepository.GetTopSpeciesAsync(cancellationToken);


        logger.LogInformation("StatisticsGetQuery executed successfully");
        return ServiceResult<StatisticsGetQueryResult>.Success(statistics);
    }
}
