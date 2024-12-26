using BioWings.Application.Features.Queries.GenusQueries;
using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Read;
public class GenusGetCountQueryHandler(IGenusRepository genusRepository, ILogger<GenusGetCountQueryHandler> logger) : IRequestHandler<GenusGetCountQuery, ServiceResult<GenusGetCountQueryResult>>
{
    public async Task<ServiceResult<GenusGetCountQueryResult>> Handle(GenusGetCountQuery request, CancellationToken cancellationToken)
    {
        var genusCount = await genusRepository.GetTotalCountAsync(cancellationToken);
        logger.LogInformation("Genus count fetched successfully");
        return ServiceResult<GenusGetCountQueryResult>.Success(new GenusGetCountQueryResult { Count=genusCount });
    }
}
