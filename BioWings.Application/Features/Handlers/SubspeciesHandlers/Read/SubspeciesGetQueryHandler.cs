using BioWings.Application.Features.Queries.SubspeciesQueries;
using BioWings.Application.Features.Results.SubspeciesResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SubspeciesHandlers.Read;
public class SubspeciesGetQueryHandler(ISubspeciesRepository subspeciesRepository, ILogger<SubspeciesGetQueryHandler> logger) : IRequestHandler<SubspeciesGetQuery, ServiceResult<IEnumerable<SubspeciesGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<SubspeciesGetQueryResult>>> Handle(SubspeciesGetQuery request, CancellationToken cancellationToken)
    {
        var subspecies = await subspeciesRepository.GetAllWithSpeciesAsync(cancellationToken);
        var result = subspecies.Select(s => new SubspeciesGetQueryResult
        {
            Id=s.Id,
            Name=s.Name,
            SpeciesId=s.SpeciesId,
            SpeciesName=s.Species.Name
        });
        return ServiceResult<IEnumerable<SubspeciesGetQueryResult>>.Success(result);
    }
}
