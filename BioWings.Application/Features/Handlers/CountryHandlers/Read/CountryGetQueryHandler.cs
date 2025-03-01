using BioWings.Application.Features.Queries.CountryQueries;
using BioWings.Application.Features.Results.CountryResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.CountryHandlers.Read;
public class CountryGetQueryHandler(ICountryRepository countryRepository, ILogger<CountryGetQueryHandler> logger) : IRequestHandler<CountryGetQuery, ServiceResult<List<CountryGetQueryResult>>>
{
    public async Task<ServiceResult<List<CountryGetQueryResult>>> Handle(CountryGetQuery request, CancellationToken cancellationToken)
    {
        var countries = await countryRepository.GetAllAsNoTracking().ToListAsync();
        var result = countries.Select(x => new CountryGetQueryResult { Id=x.Id, Name=x.Name }).ToList();
        logger.LogInformation($"Retrieved {result.Count} countries successfully");
        return ServiceResult<List<CountryGetQueryResult>>.Success(result);
    }
}
