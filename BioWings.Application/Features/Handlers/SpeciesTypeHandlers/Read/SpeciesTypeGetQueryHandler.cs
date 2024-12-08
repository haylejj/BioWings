using BioWings.Application.Features.Queries.SpeciesTypeQueries;
using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesTypeHandlers.Read;
public class SpeciesTypeGetQueryHandler(ISpeciesTypeRepository speciesTypeRepository, ILogger<SpeciesTypeGetQueryHandler> logger) : IRequestHandler<SpeciesTypeGetQuery, ServiceResult<IEnumerable<SpeciesTypeGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<SpeciesTypeGetQueryResult>>> Handle(SpeciesTypeGetQuery request, CancellationToken cancellationToken)
    {
        var speciesTypes = await speciesTypeRepository.GetAllAsync();
        var result = speciesTypes.Select(x => new SpeciesTypeGetQueryResult
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        });
        logger.LogInformation("SpeciesTypeGetPagedQueryHandler: {0} species types found", result.Count());
        return ServiceResult<IEnumerable<SpeciesTypeGetQueryResult>>.Success(result);
    }
}
