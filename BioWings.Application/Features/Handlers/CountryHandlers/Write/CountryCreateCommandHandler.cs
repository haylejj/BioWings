using BioWings.Application.Features.Commands.CountryCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.CountryHandlers.Write;
public class CountryCreateCommandHandler(ICountryRepository countryRepository, IUnitOfWork unitOfWork, ILogger<CountryCreateCommandHandler> logger) : IRequestHandler<CountryCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CountryCreateCommand request, CancellationToken cancellationToken)
    {
        if (request==null)
        {
            logger.LogError("CountryCreateCommand request is null.");
            return ServiceResult.Error("CountryCreateCommand request is null.");
        }
        var country = new Country
        {
            Name = request.Name
        };
        await countryRepository.AddAsync(country, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Country created with ID: {CountryId}", country.Id);
        return ServiceResult.SuccessAsCreated($"api/Country/{country.Id}");
    }
}

