using BioWings.Application.Features.Results.StatisticResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.StatisticsQueries;
public class StatisticsGetQuery : IRequest<ServiceResult<StatisticsGetQueryResult>>
{
}
