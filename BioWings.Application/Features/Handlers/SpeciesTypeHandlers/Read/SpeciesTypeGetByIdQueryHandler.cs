using BioWings.Application.Features.Queries.SpeciesTypeQueries;
using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Read;
public class SpeciesTypeGetByIdQueryHandler(ISpeciesTypeRepository speciesTypeRepository, ILogger<SpeciesTypeGetByIdQueryHandler> logger) : IRequestHandler<SpeciesTypeGetByIdQuery, ServiceResult<SpeciesTypeGetByIdQueryResult>>
{
    public async Task<ServiceResult<SpeciesTypeGetByIdQueryResult>> Handle(SpeciesTypeGetByIdQuery request, CancellationToken cancellationToken)
    {
        var speciesType = await speciesTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciesType == null)
        {
            logger.LogWarning("SpeciesType not found");
            return ServiceResult<SpeciesTypeGetByIdQueryResult>.Error("SpeciesType not found", HttpStatusCode.NotFound);
        }
        var result = new SpeciesTypeGetByIdQueryResult
        {
            Id = speciesType.Id,
            Name = speciesType.Name,
            Description = speciesType.Description
        };
        logger.LogInformation("SpeciesType found successfully");
        return ServiceResult<SpeciesTypeGetByIdQueryResult>.Success(result);
    }
}
