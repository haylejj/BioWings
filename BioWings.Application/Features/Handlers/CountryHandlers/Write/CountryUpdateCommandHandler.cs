using BioWings.Application.Features.Commands.CountryCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.CountryHandlers.Write;

public class CountryUpdateCommandHandler(ICountryRepository countryRepository, IUnitOfWork unitOfWork, ILogger<CountryUpdateCommandHandler> logger) : IRequestHandler<CountryUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(CountryUpdateCommand request, CancellationToken cancellationToken)
    {
        var country = await countryRepository.GetByIdAsync(request.Id);
        if (country == null)
        {
            logger.LogWarning("Country not found with ID: {CountryId}", request.Id);
            return ServiceResult.Error("Country not found", HttpStatusCode.NotFound);
        }
        country.Name = request.Name;
        countryRepository.Update(country);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Country updated successfully with ID: {CountryId}", country.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}

