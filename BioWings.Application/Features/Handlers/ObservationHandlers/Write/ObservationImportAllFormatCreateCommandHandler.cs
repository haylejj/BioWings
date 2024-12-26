using BioWings.Application.DTOs.ImportDtos;
using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Keys;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationImportAllFormatCreateCommandHandler(ILogger<ObservationImportAllFormatCreateCommandHandler> logger,IGeocodingService geocodingService, IUnitOfWork unitOfWork, IExcelImportService excelImportService, IObservationRepository observationRepository, ISubspeciesRepository subspeciesRepository, ISpeciesRepository speciesRepository, ILocationRepository locationRepository, IProvinceRepository provinceRepository, IObserverRepository observerRepository, IAuthorityRepository authorityRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository) : IRequestHandler<ObservationImportAllFormatCreateCommand, ServiceResult>
{
    private Dictionary<string, Family> _familyCache = new();
    private Dictionary<GenusKey, Genus> _genusCache = new();
    private Dictionary<string, Observer> _observerCache = new();
    private Dictionary<AuthorityKey, Authority> _authorityCache = new();
    private Dictionary<LocationKey, Location> _locationCache = new();
    private Dictionary<SpeciesKey, Species> _speciesCache = new();
    private DbContext context = unitOfWork.GetContext();
    public async Task<ServiceResult> Handle(ObservationImportAllFormatCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var importDtos = await excelImportService.ImportFromExcelAsync(request.File);
            logger.LogInformation($"Excel'den {importDtos.Count} satır okundu.");
            const int batchSize = 1500;
            var totalProcessed = 0;
            // Batch fetching existing data
            var existingAuthorities = authorityRepository.GetAllAsNoTracking();
            _authorityCache = existingAuthorities.ToDictionary(
                a => new AuthorityKey { Name = a.Name, Year = a.Year.GetValueOrDefault() },
                a => a
            );

            var existingFamilies = familyRepository.GetAllAsNoTracking();
            _familyCache = existingFamilies.ToDictionary(f => f.Name);

            //var existingGenera =  genusRepository.GetAllAsNoTracking();
            //_genusCache = existingGenera.ToDictionary(x=> new GenusKey { FamilyId=x.FamilyId,GenusName=x.Name},x=> x);
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
                    // Handle duplicates, e.g., log a warning or merge duplicates
                    logger.LogWarning($"Duplicate GenusKey found: {genusKey.GenusName}, {genusKey.FamilyId}");
                }
            }

            _genusCache = genusDict;

            var existingObservers = observerRepository.GetAllAsNoTracking();
            _observerCache = existingObservers.ToDictionary(o => o.FullName);

            var existingLocations = locationRepository.GetAllAsNoTracking();
            _locationCache = existingLocations.ToDictionary(l => new LocationKey { Latitude = l.Latitude, Longitude = l.Longitude }, l => l);

            var existingSpecies = speciesRepository.GetAllAsNoTracking();
            _speciesCache = existingSpecies.ToDictionary(
                s => new SpeciesKey { Name = s.Name, GenusId = s.GenusId, AuthorityId = s.AuthorityId },
                s => s
            );

            await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Excel verilerini gruplama
                    var groupedData = importDtos.GroupBy(x => new
                    {
                        x.AuthorityName,
                        x.AuthorityYear,
                        x.FamilyName,
                        x.GenusName,
                        x.ProvinceName,
                        x.ProvinceCode,
                        x.ObserverFullName
                    }).ToList();

                    foreach (var group in groupedData)
                    {
                        var authorityKey = new AuthorityKey { Name = group.Key.AuthorityName, Year = group.Key.AuthorityYear };
                        var authority = !string.IsNullOrEmpty(group.Key.AuthorityName) && _authorityCache.TryGetValue(authorityKey, out var existingAuthority)
                            ? existingAuthority
                            : await GetOrCreateAuthorityAsync(group.Key.AuthorityName, group.Key.AuthorityYear, cancellationToken);

                        var family = !string.IsNullOrEmpty(group.Key.FamilyName) && _familyCache.TryGetValue(group.Key.FamilyName, out var existingFamily)
                            ? existingFamily
                            : await GetOrCreateFamilyAsync(group.Key.FamilyName, cancellationToken);

                        var genusKey = new GenusKey { GenusName = group.Key.GenusName, FamilyId = family?.Id };
                        var genus = !string.IsNullOrEmpty(group.Key.GenusName) && _genusCache.TryGetValue(genusKey, out var existingGenus)
                            ? existingGenus
                            : await GetOrCreateGenusAsync(group.Key.GenusName, family?.Id, cancellationToken);

                        var observer = !string.IsNullOrEmpty(group.Key.ObserverFullName) && _observerCache.TryGetValue(group.Key.ObserverFullName, out var existingObserver)
                            ? existingObserver
                            : await GetOrCreateObserverAsync(group.Key.ObserverFullName, cancellationToken);



                        var batchObservations = new List<Observation>();

                        foreach (var dto in group)
                        {
                            //var species = await speciesRepository.GetOrCreateSpeciesAsync(dto, genus?.Id, authority?.Id, cancellationToken);

                            //// Location işlemi
                            //var location = await locationRepository.GetOrCreateLocationAsync(dto, group.Key.ProvinceCode, cancellationToken);

                            var speciesKey = new SpeciesKey { Name = dto.SpeciesName, GenusId = genus?.Id, AuthorityId = authority?.Id };
                            var species = !string.IsNullOrEmpty(dto.SpeciesName) && _speciesCache.TryGetValue(speciesKey, out var existingSpecies)
                                ? existingSpecies
                                : await CreateBatchSpeciesAsync(dto, genus?.Id, authority?.Id, cancellationToken);

                            var locationKey = new LocationKey { Latitude = dto.Latitude, Longitude = dto.Longitude };
                            var location = _locationCache.TryGetValue(locationKey, out var existingLocation)
                                ? existingLocation
                                : await CreateBatchLocationAsync(dto, group.Key.ProvinceCode, cancellationToken);

                            var observation = new Observation
                            {
                                SpeciesId = species.Id,
                                LocationId = location.Id,
                                ObserverId = observer?.Id,
                                Sex = dto.Sex,
                                ObservationDate = dto.ObservationDate,
                                LifeStage = dto.LifeStage,
                                NumberSeen = dto.NumberSeen,
                                Notes = dto.Notes,
                                Source = dto.Source,
                                LocationInfo = dto.LocationInfo
                            };
                            batchObservations.Add(observation);

                            // Batch size'a ulaşınca bulk insert yap
                            if (batchObservations.Count >= batchSize)
                            {
                                await observationRepository.BulkInsertObservationsAsync(batchObservations, cancellationToken);
                                totalProcessed += batchObservations.Count;
                                logger.LogInformation($"Batch processed: {totalProcessed}/{importDtos.Count}");
                                batchObservations.Clear();
                                unitOfWork.GetContext().ChangeTracker.Clear();
                            }
                        }

                        // Kalan observations varsa onları da ekle
                        if (batchObservations.Any())
                        {
                            await observationRepository.BulkInsertObservationsAsync(batchObservations, cancellationToken);
                            totalProcessed += batchObservations.Count;
                            logger.LogInformation($"Batch processed: {totalProcessed}/{importDtos.Count}");
                        }
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return ServiceResult.Success();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });

            // Duplicate temizleme işlemi
            await CleanDuplicateObservations(cancellationToken);
            logger.LogInformation($"Toplam işlem süresi: {stopwatch.Elapsed.TotalMinutes:F2} dakika");
            return ServiceResult.Success();
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
    private async Task<Observer> GetOrCreateObserverAsync(string fullName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            return null;
        }
        var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var surname = nameParts.Length > 1 ? nameParts[^1] : string.Empty;
        var name = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : fullName;

        var observer = new Observer
        {
            Name = name,
            Surname = surname,
            FullName = fullName
        };
        await observerRepository.AddAsync(observer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        _observerCache[fullName] = observer;
        return observer;
    }
    private async Task<Location> CreateBatchLocationAsync(ImportCreateDto importCreateDto, int? provinceId, CancellationToken cancellationToken)
    {
        if (importCreateDto.ProvinceName == null && importCreateDto.Latitude != 0 && importCreateDto.Longitude != 0)
        {
            var provinceCode = await geocodingService.GetProvinceAsync(importCreateDto.Latitude, importCreateDto.Longitude);
            if (provinceCode == null)
            {
                logger.LogWarning($"Province bulunamadı: Lat={importCreateDto.Latitude}, Lon={importCreateDto.Longitude}");
            }
            else
            {
                importCreateDto.ProvinceCode = int.Parse(provinceCode);
            }

        }
        var locationKey = new LocationKey { Latitude = importCreateDto.Latitude, Longitude = importCreateDto.Longitude };
        var location = new Location
        {
            Latitude = importCreateDto.Latitude,
            Longitude = importCreateDto.Longitude,
            ProvinceId = provinceId,
            Altitude1 = importCreateDto.Altitude1,
            Altitude2 = importCreateDto.Altitude2,
            CoordinatePrecisionLevel = importCreateDto.CoordinatePrecisionLevel,
            DecimalDegrees = importCreateDto.DecimalDegrees,
            DecimalMinutes = importCreateDto.DecimalMinutes,
            DegreesMinutesSeconds= importCreateDto.DegreesMinutesSeconds,
            UtmReference = importCreateDto.UtmReference,
            UtmCoordinates = importCreateDto.UtmCoordinates,
            MgrsCoordinates = importCreateDto.MgrsCoordinates,
            SquareLatitude = importCreateDto.SquareLatitude,
            SquareRef = importCreateDto.SquareRef,
            SquareLongitude = importCreateDto.SquareLongitude,
        };
        await locationRepository.AddAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        _locationCache[locationKey] = location;
        return location;
    }
    private async Task<Species> CreateBatchSpeciesAsync(ImportCreateDto ımportCreateDto, int? genusId, int? authorityId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ımportCreateDto.SpeciesName))
        {
            return null;
        }
        var speciesKey = new SpeciesKey { Name = ımportCreateDto.SpeciesName, GenusId = genusId, AuthorityId = authorityId };
        var species = new Species
        {
            AuthorityId=authorityId,
            GenusId=genusId,
            FullName=ımportCreateDto.FullName,
            EnglishName=ımportCreateDto.EnglishName,
            EUName=ımportCreateDto.EUName,
            HesselbarthName=ımportCreateDto.HesselbarthName,
            KocakName=ımportCreateDto.KocakName,
            ScientificName=ımportCreateDto.ScientificName,
            Name=ımportCreateDto.SpeciesName,
            TurkishName=ımportCreateDto.TurkishName,
            TurkishNamesTrakel=ımportCreateDto.TurkishNamesTrakel,
            Trakel=ımportCreateDto.Trakel,
        };
        await speciesRepository.AddAsync(species, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        _speciesCache[speciesKey] = species;
        return species;
    }
    private async Task CleanDuplicateObservations(CancellationToken cancellationToken)
    {
        //const int batchSize = 1500;
        //var duplicateIds = await observationRepository.GetAllDuplicateObservationIdsAsync(cancellationToken);

        //if (duplicateIds.Any())
        //{
        //    for (int i = 0; i < duplicateIds.Count; i += batchSize)
        //    {
        //        var batchIds = duplicateIds.Skip(i).Take(batchSize).ToList();
        //        unitOfWork.GetContext().ChangeTracker.Clear();
        //        await observationRepository.RemoveDuplicatesAsync(batchIds, cancellationToken);
        //        await unitOfWork.SaveChangesAsync(cancellationToken);
        //    }
        //    logger.LogInformation($"Removed {duplicateIds.Count} duplicate observations");
        //}
        await observationRepository.RemoveDuplicatesWithProcedureAsync("CALL CleanDuplicateObservations()", cancellationToken);
        //await observationRepository.RemoveDuplicateObservationsWithEfCoreAsync(cancellationToken);
        logger.LogInformation($"Removed  duplicate observations successfully.");
    }
}