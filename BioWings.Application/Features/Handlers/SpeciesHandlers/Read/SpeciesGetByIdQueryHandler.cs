using BioWings.Application.Features.Queries.SpeciesQueries;
using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Read;
public class SpeciesGetByIdQueryHandler(ISpeciesRepository speciesRepository, ILogger<SpeciesGetByIdQueryHandler> logger) : IRequestHandler<SpeciesGetByIdQuery, ServiceResult<SpeciesGetByIdQueryResult>>
{
    public async Task<ServiceResult<SpeciesGetByIdQueryResult>> Handle(SpeciesGetByIdQuery request, CancellationToken cancellationToken)
    {
        var species = await speciesRepository.GetByIdWithGenus_Authority_SpeciesTypeAsync(request.Id, cancellationToken);
        if (species is null)
        {
            logger.LogError("Species with ID {SpeciesId} was not found", request.Id);
            return ServiceResult<SpeciesGetByIdQueryResult>.Error($"Species with ID {request.Id} was not found", System.Net.HttpStatusCode.NotFound);
        }

        var speciesResult = new SpeciesGetByIdQueryResult
        {
            Id = species.Id,
            AuthorityId = species.AuthorityId,
            AuthorityName = species.Authority.Name,
            GenusId = species.GenusId,
            GenusName = species.Genus.Name,
            SpeciesTypeId = species.SpeciesTypeId,
            SpeciesTypeName = species.SpeciesType.Name,
            ScientificName = species.ScientificName,
            Name = species.Name,
            EUName = species.EUName,
            FullName = species.FullName,
            TurkishName = species.TurkishName,
            EnglishName = species.EnglishName,
            TurkishNamesTrakel = species.TurkishNamesTrakel,
            Trakel = species.Trakel
        };

        logger.LogInformation("Retrieved species {SpeciesName} (ID: {SpeciesId}) of genus {GenusName}",
            species.Name,
            species.Id,
            species.Genus.Name);

        return ServiceResult<SpeciesGetByIdQueryResult>.Success(speciesResult);
    }
}
