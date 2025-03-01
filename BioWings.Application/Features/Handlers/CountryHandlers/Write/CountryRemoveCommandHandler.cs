using BioWings.Application.Features.Commands.CountryCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.CountryHandlers.Write;

public class CountryRemoveCommandHandler(ICountryRepository countryRepository, IUnitOfWork unitOfWork, ILogger<CountryRemoveCommandHandler> logger) : IRequestHandler<CountryRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CountryRemoveCommand request, CancellationToken cancellationToken)
    {
        var country = await countryRepository.GetByIdAsync(request.Id);
        if (country == null)
        {
            logger.LogWarning("Country not found with ID: {CountryId}", request.Id);
            return ServiceResult.Error("Country not found", HttpStatusCode.NotFound);
        }
        countryRepository.Remove(country);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Country removed successfully with ID: {CountryId}", country.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}

