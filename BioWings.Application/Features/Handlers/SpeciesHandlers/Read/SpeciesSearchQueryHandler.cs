using BioWings.Application.Features.Queries.SpeciesQueries;
using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Read;
public class SpeciesSearchQueryHandler(ISpeciesRepository speciesRepository,ILogger<SpeciesSearchQueryHandler> logger) : IRequestHandler<SpeciesSearchQuery, ServiceResult<PaginatedList<SpeciesSearchQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<SpeciesSearchQueryResult>>> Handle(SpeciesSearchQuery request, CancellationToken cancellationToken)
    {
        var species = speciesRepository.GetAllAsQueryable();
        species=species.Include(s => s.Genus).ThenInclude(s => s.Family).Include(s => s.Authority).AsQueryable();
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            species = species.Where(s => s.Name.ToLower().Contains(searchTerm) || s.ScientificName.ToLower().Contains(searchTerm) || s.FullName.ToLower().Contains(searchTerm) || s.TurkishName.ToLower().Contains(searchTerm) || s.EnglishName.ToLower().Contains(searchTerm) || s.KocakName.ToLower().Contains(searchTerm) || s.HesselbarthName.ToLower().Contains(searchTerm));
        }
        var totalCount = await speciesRepository.GetTotalCountAsync(cancellationToken);
        var items = await species
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SpeciesSearchQueryResult
            {
                Id = x.Id,
                AuthorityId = x.AuthorityId,
                AuthorityName = x.Authority.Name,
                GenusId = x.GenusId,
                GenusName = x.Genus.Name,
                FamilyId = x.Genus.FamilyId,
                FamilyName = x.Genus.Family.Name,
                ScientificName = x.ScientificName,
                Name = x.Name,
                EUName = x.EUName,
                FullName = x.FullName,
                TurkishName = x.TurkishName,
                EnglishName = x.EnglishName,
                TurkishNamesTrakel = x.TurkishNamesTrakel,
                Trakel = x.Trakel,
                KocakName = x.KocakName,
                HesselbarthName = x.HesselbarthName
            }).ToListAsync(cancellationToken);
        var paginatedResult = new PaginatedList<SpeciesSearchQueryResult>(items,totalCount, request.PageNumber, request.PageSize);
        logger.LogInformation("Species are filtered and fetched successfully.");
        return ServiceResult<PaginatedList<SpeciesSearchQueryResult>>.Success(paginatedResult);
    }
}
