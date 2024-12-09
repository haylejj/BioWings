using BioWings.Application.Features.Queries.SubspeciesQueries;
using BioWings.Application.Features.Results.SubspeciesResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Read;

public class SubspeciesGetByIdQueryHandler(ISubspeciesRepository subspeciesRepository, ILogger<SubspeciesGetQueryHandler> logger) : IRequestHandler<SubspeciesGetByIdQuery, ServiceResult<SubspeciesGetByIdQueryResult>>
{
    public async Task<ServiceResult<SubspeciesGetByIdQueryResult>> Handle(SubspeciesGetByIdQuery request, CancellationToken cancellationToken)
    {
        var subspecies = await subspeciesRepository.GetByIdWithSpeciesAsync(request.Id, cancellationToken);
        if (subspecies == null)
        {
            logger.LogWarning("Subspecies not found");
            return ServiceResult<SubspeciesGetByIdQueryResult>.Error("Subspecies not found", System.Net.HttpStatusCode.NotFound);
        }
        var result = new SubspeciesGetByIdQueryResult
        {
            Id = subspecies.Id,
            Name = subspecies.Name,
            SpeciesId = subspecies.SpeciesId,
            SpeciesName = subspecies.Species.Name
        };
        return ServiceResult<SubspeciesGetByIdQueryResult>.Success(result);

    }
}
