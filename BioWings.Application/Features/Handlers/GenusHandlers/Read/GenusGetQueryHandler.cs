using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Read;
public class GenusGetQueryHandler(IGenusRepository genusRepository, ILogger<GenusGetQueryHandler> logger) : IRequestHandler<GenusGetQuery, ServiceResult<IEnumerable<GenusGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<GenusGetQueryResult>>> Handle(GenusGetQuery request, CancellationToken cancellationToken)
    {
        var genera = await genusRepository.GetAllAsync(cancellationToken);
        if (genera == null || !genera.Any())
        {
            logger.LogWarning("No genera found");
            return ServiceResult<IEnumerable<GenusGetQueryResult>>.Error("No genera found", HttpStatusCode.NotFound);
        }
        var result = genera.Select(g => new GenusGetQueryResult
        {
            Id = g.Id,
            Name = g.Name
        });
        logger.LogInformation("Genera found successfully");
        return ServiceResult<IEnumerable<GenusGetQueryResult>>.Success(result);
    }
}
