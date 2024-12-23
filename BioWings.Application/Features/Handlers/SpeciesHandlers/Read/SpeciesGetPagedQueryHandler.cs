using BioWings.Application.Features.Queries.SpeciesQueries;
using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Read;
public class SpeciesGetPagedQueryHandler(ISpeciesRepository speciesRepository, ILogger<SpeciesGetPagedQueryHandler> logger) : IRequestHandler<SpeciesGetPagedQuery, ServiceResult<PaginatedList<SpeciesGetPagedQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<SpeciesGetPagedQueryResult>>> Handle(SpeciesGetPagedQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);
        var totalCount = await speciesRepository.GetTotalCountAsync(cancellationToken);

        var species = await speciesRepository.GetPagedAsQueryable(request.PageNumber, request.PageSize).Include(x => x.Genus).Include(x => x.Authority).ToListAsync(cancellationToken);
        var result = species.Select(s => new SpeciesGetPagedQueryResult
        {
            Id = s.Id,
            GenusId = s.GenusId,
            GenusName = s.Genus.Name,
            AuthorityId = s.AuthorityId,
            AuthorityName = s.Authority.Name,
            FullName = s.FullName,
            EnglishName = s.EnglishName,
            EUName = s.EUName,
            Name = s.Name,
            ScientificName = s.ScientificName,
            TurkishName = s.TurkishName,
            TurkishNamesTrakel = s.TurkishNamesTrakel,
            Trakel=s.Trakel,
            HesselbarthName = s.HesselbarthName,
            KocakName = s.KocakName
        });
        var paginatedResult = new PaginatedList<SpeciesGetPagedQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize);
        logger.LogInformation("Species found successfully with paged");
        return ServiceResult<PaginatedList<SpeciesGetPagedQueryResult>>.Success(paginatedResult);
    }
}
