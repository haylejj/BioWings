using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Read;
internal class GenusGetByIdQueryHandler(IGenusRepository genusRepository, ILogger<GenusGetByIdQueryHandler> logger) : IRequestHandler<GenusGetByIdQuery, ServiceResult<GenusGetByIdQueryResult>>
{
    public async Task<ServiceResult<GenusGetByIdQueryResult>> Handle(GenusGetByIdQuery request, CancellationToken cancellationToken)
    {
        var genus = await genusRepository.GetByIdAsync(request.Id, cancellationToken);
        if (genus == null)
        {
            logger.LogWarning("Genus not found");
            return ServiceResult<GenusGetByIdQueryResult>.Error("Genus not found", HttpStatusCode.NotFound);
        }
        var result = new GenusGetByIdQueryResult
        {
            Id = genus.Id,
            Name = genus.Name
        };
        logger.LogInformation("Genus found successfully");
        return ServiceResult<GenusGetByIdQueryResult>.Success(result);
    }
}
