using BioWings.Application.Features.Commands.CountryCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.CountryHandlers.Write;

public class CountryCreateRangeCommandHandler(ICountryRepository countryRepository, IUnitOfWork unitOfWork, ILogger<CountryCreateRangeCommandHandler> logger) : IRequestHandler<CountryCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CountryCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request.Countries == null || !request.Countries.Any())
        {
            logger.LogWarning("No countries found to create");
            return ServiceResult.Error("No countries found to create");
        }
        var countries = request.Countries.Select(c => new Country
        {
            Name = c.Name
        });
        await countryRepository.AddRangeAsync(countries, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Countries created successfully");
        return ServiceResult.SuccessAsCreated("/api/Countries/Range");
    }
}

