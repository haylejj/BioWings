using BioWings.Application.DTOs.ImportDtos;
using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Keys;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesImportCreateCommandHandler(ISpeciesRepository speciesRepository,IUnitOfWork unitOfWork,IAuthorityRepository authorityRepository,IGenusRepository genusRepository,IFamilyRepository familyRepository,ILogger<SpeciesImportCreateCommandHandler> logger,IExcelImportService excelImportService) : IRequestHandler<SpeciesImportCreateCommand, ServiceResult>
{
    private Dictionary<string, Family> _familyCache = new();
    private Dictionary<GenusKey, Genus> _genusCache = new();
    private Dictionary<AuthorityKey, Authority> _authorityCache = new();
    private Dictionary<SpeciesKey, Species> _speciesCache = new();
    private DbContext context = unitOfWork.GetContext();
    public async Task<ServiceResult> Handle(SpeciesImportCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var importDtos = excelImportService.ImportSpeciesFromExcel(request.File);
            logger.LogInformation($"Excel'den {importDtos.Count} satır okundu.");

            const int batchSize = 500;
            var totalProcessed = 0;

            // Mevcut verileri cache'le
            var existingAuthorities = authorityRepository.GetAllAsNoTracking();
            _authorityCache = existingAuthorities.ToDictionary(
                a => new AuthorityKey { Name = a.Name, Year = a.Year.GetValueOrDefault() },
                a => a
            );

            var existingFamilies = familyRepository.GetAllAsNoTracking();
            _familyCache = existingFamilies.ToDictionary(f => f.Name);

            var existingGenera = genusRepository.GetAllAsNoTracking();
            var genusDict = new Dictionary<GenusKey, Genus>();
            foreach (var genus in existingGenera)
            {
                var genusKey = new GenusKey { GenusName = genus.Name, FamilyId = genus.FamilyId };
                if (!genusDict.ContainsKey(genusKey))
                {
                    genusDict[genusKey] = genus;
                }
                else
                {
                    logger.LogWarning($"Duplicate GenusKey found: {genusKey.GenusName}, {genusKey.FamilyId}");
                }
            }
            _genusCache = genusDict;

            var existingSpecies = speciesRepository.GetAllAsNoTracking();
            _speciesCache = existingSpecies.ToDictionary(
                s => new SpeciesKey { Name = s.Name, GenusId = s.GenusId, AuthorityId = s.AuthorityId },
                s => s
            );

            var result = await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var groupedData = importDtos.GroupBy(x => new
                    {
                        x.AuthorityName,
                        x.AuthorityYear,
                        x.FamilyName,
                        x.GenusName
                    }).ToList();

                    var batchSpecies = new List<Species>();

                    foreach (var group in groupedData)
                    {
                        var authorityKey = new AuthorityKey { Name = group.Key.AuthorityName, Year = group.Key.AuthorityYear.Value };
                        var authority = !string.IsNullOrEmpty(group.Key.AuthorityName) && _authorityCache.TryGetValue(authorityKey, out var existingAuthority)
                            ? existingAuthority
                            : await GetOrCreateAuthorityAsync(group.Key.AuthorityName, group.Key.AuthorityYear.Value, cancellationToken);

                        var family = !string.IsNullOrEmpty(group.Key.FamilyName) && _familyCache.TryGetValue(group.Key.FamilyName, out var existingFamily)
                            ? existingFamily
                            : await GetOrCreateFamilyAsync(group.Key.FamilyName, cancellationToken);

                        var genusKey = new GenusKey { GenusName = group.Key.GenusName, FamilyId = family?.Id };
                        var genus = !string.IsNullOrEmpty(group.Key.GenusName) && _genusCache.TryGetValue(genusKey, out var existingGenus)
                            ? existingGenus
                            : await GetOrCreateGenusAsync(group.Key.GenusName, family?.Id, cancellationToken);

                        foreach (var dto in group)
                        {
                            var speciesKey = new SpeciesKey
                            {
                                Name = dto.SpeciesName,
                                GenusId = genus?.Id,
                                AuthorityId = authority?.Id
                            };

                            if (!_speciesCache.TryGetValue(speciesKey, out var existingSpecies))
                            {
                                var species = new Species
                                {
                                    AuthorityId = authority?.Id,
                                    GenusId = genus?.Id,
                                    FullName = dto.FullName,
                                    EnglishName = dto.EnglishName,
                                    EUName = dto.EUName,
                                    HesselbarthName = dto.HesselbarthName,
                                    KocakName = dto.KocakName,
                                    ScientificName = dto.ScientificName,
                                    Name = dto.SpeciesName,
                                    TurkishName = dto.TurkishName,
                                    TurkishNamesTrakel = dto.TurkishNamesTrakel,
                                    Trakel = dto.Trakel
                                };

                                if (!string.IsNullOrWhiteSpace(species.Name))
                                {
                                    batchSpecies.Add(species);
                                    _speciesCache[speciesKey] = species;
                                }

                                if (batchSpecies.Count >= batchSize)
                                {
                                    await speciesRepository.AddRangeAsync(batchSpecies, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken); 
                                    totalProcessed += batchSpecies.Count;
                                    logger.LogInformation($"Batch processed: {totalProcessed}/{importDtos.Count}");
                                    batchSpecies.Clear();
                                    unitOfWork.GetContext().ChangeTracker.Clear();
                                }
                            }
                        }
                    }

                    if (batchSpecies.Any())
                    {
                        await speciesRepository.AddRangeAsync(batchSpecies, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken); // Eklenmeli
                        totalProcessed += batchSpecies.Count;
                        logger.LogInformation($"Batch processed: {totalProcessed}/{importDtos.Count}");
                    }

                    await transaction.CommitAsync(cancellationToken);
                    logger.LogInformation($"Toplam işlem süresi: {stopwatch.Elapsed.TotalMinutes:F2} dakika");
                    return ServiceResult.SuccessAsCreated("api/Species/Import");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import işlemi sırasında hata oluştu");
            return ServiceResult.Error($"Import işlemi sırasında hata oluştu: {ex.Message}");
        }
    }
    private async Task<Authority> GetOrCreateAuthorityAsync(string name, int year, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name) || year == null)
        {
            return null;
        }
        var authorityKey = new AuthorityKey { Name = name, Year = year };
        var authority = new Authority { Name = name, Year = year };
        await authorityRepository.AddAsync(authority, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        _authorityCache[authorityKey] = authority;
        return authority;
    }
    private async Task<Family> GetOrCreateFamilyAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var family = new Family { Name = name };
        await familyRepository.AddAsync(family, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        _familyCache[name] = family;
        return family;
    }
    private async Task<Genus> GetOrCreateGenusAsync(string name, int? familyId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(name) && familyId == null)
        {
            return null;
        }
        var genusKey = new GenusKey { GenusName = name, FamilyId = familyId };
        if (!_genusCache.TryGetValue(genusKey, out var genus))
        {
            genus = new Genus { Name = name, FamilyId = familyId };
            await genusRepository.AddAsync(genus, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _genusCache[genusKey] = genus;
        }
        return genus;
    }
}
