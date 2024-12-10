using BioWings.Application.Features.Queries.SpeciesQueries;
using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Read;
public class SpeciesGetQueryHandler(ISpeciesRepository speciesRepository, ILogger<SpeciesGetQueryHandler> logger) : IRequestHandler<SpeciesGetQuery, ServiceResult<IEnumerable<SpeciesGetQueryResult>>>
{
    public async Task<ServiceResult<IEnumerable<SpeciesGetQueryResult>>> Handle(SpeciesGetQuery request, CancellationToken cancellationToken)
    {
        var species = await speciesRepository.GetAllAsQueryable().Include(x => x.Authority).Include(x => x.Genus).Include(x => x.SpeciesType).ToListAsync(cancellationToken);
        var speciesResult = species.Select(x => new SpeciesGetQueryResult
        {
            Id = x.Id,
            AuthorityId = x.AuthorityId,
            AuthorityName = x.Authority.Name,
            GenusId = x.GenusId,
            GenusName = x.Genus.Name,
            SpeciesTypeId = x.SpeciesTypeId,
            SpeciesTypeName = x.SpeciesType.Name,
            ScientificName = x.ScientificName,
            Name = x.Name,
            EUName = x.EUName,
            FullName = x.FullName,
            TurkishName = x.TurkishName,
            EnglishName = x.EnglishName,
            TurkishNamesTrakel = x.TurkishNamesTrakel,
            Trakel = x.Trakel,
            HesselbarthName = x.HesselbarthName,
            KocakName = x.KocakName
        });
        return ServiceResult<IEnumerable<SpeciesGetQueryResult>>.Success(speciesResult);
    }
}
