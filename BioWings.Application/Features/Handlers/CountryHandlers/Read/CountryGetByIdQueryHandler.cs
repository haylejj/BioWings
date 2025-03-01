using BioWings.Application.Features.Queries.CountryQueries;
using BioWings.Application.Features.Results.CountryResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.CountryHandlers.Read;
public class CountryGetByIdQueryHandler(ICountryRepository countryRepository, ILogger<CountryGetByIdQueryHandler> logger) : IRequestHandler<CountryGetByIdQuery, ServiceResult<CountryGetByIdQueryResult>>
{
    public async Task<ServiceResult<CountryGetByIdQueryResult>> Handle(CountryGetByIdQuery request, CancellationToken cancellationToken)
    {
        var country = await countryRepository.GetByIdAsync(request.Id);
        if (country == null)
        {
            logger.LogError($"Country with id {request.Id} not found.");
            return ServiceResult<CountryGetByIdQueryResult>.Error($"Country with id {request.Id} not found.", System.Net.HttpStatusCode.NotFound);
        }
        var result = new CountryGetByIdQueryResult
        {
            Id=country.Id,
            Name=country.Name
        };
        logger.LogInformation($"Country with id {request.Id} found.");
        return ServiceResult<CountryGetByIdQueryResult>.Success(result);
    }
}
