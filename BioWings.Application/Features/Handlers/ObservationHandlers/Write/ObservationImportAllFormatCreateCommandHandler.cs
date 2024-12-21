using BioWings.Application.DTOs.ImportDtos;
using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationImportAllFormatCreateCommandHandler(ILogger<ObservationImportAllFormatCreateCommandHandler> logger, IUnitOfWork unitOfWork, IExcelImportService excelImportService, IObservationRepository observationRepository, ISubspeciesRepository subspeciesRepository, ISpeciesRepository speciesRepository, ILocationRepository locationRepository, IProvinceRepository provinceRepository, IObserverRepository observerRepository, IAuthorityRepository authorityRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository) : IRequestHandler<ObservationImportAllFormatCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationImportAllFormatCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var importDtos = excelImportService.ImportFromExcel(request.File);
            logger.LogInformation($"Excel'den {importDtos.Count} satır okundu.");

            const int batchSize = 1000;
            var totalProcessed = 0;
            ServiceResult result = null;

            await unitOfWork.GetContext().Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = await unitOfWork.GetContext().Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    for (int i = 0; i < importDtos.Count; i += batchSize)
                    {
                        var batch = importDtos.Skip(i).Take(batchSize).ToList();
                        var batchAuthorities = new List<Authority>();
                        var batchFamilies = new List<Family>();
                        var batchGenera = new List<Genus>();
                        var batchSpecies = new List<Species>();
                        var batchProvinces = new List<Province>();
                        var batchLocations = new List<Location>();
                        var batchObservers = new List<Observer>();
                        var batchObservations = new List<Observation>();

                        foreach (var dto in batch)
                        {
                            try
                            {
                                // Authority işlemi
                                var authority = await GetOrCreateAuthorityWithBatch(dto, batchAuthorities, cancellationToken);
                                if (batchAuthorities.Any())
                                {
                                    await authorityRepository.AddRangeAsync(batchAuthorities, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchAuthorities.Clear();
                                }

                                // Family işlemi
                                var family = await GetOrCreateFamilyWithBatch(dto, batchFamilies, cancellationToken);
                                if (batchFamilies.Any())
                                {
                                    await familyRepository.AddRangeAsync(batchFamilies, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchFamilies.Clear();
                                }

                                // Genus işlemi
                                var genus = await GetOrCreateGenusWithBatch(dto, family?.Id, batchGenera, cancellationToken);
                                if (batchGenera.Any())
                                {
                                    await genusRepository.AddRangeAsync(batchGenera, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchGenera.Clear();
                                }

                                // Species işlemi
                                var species = await GetOrCreateSpeciesWithBatch(dto, genus?.Id, authority?.Id, batchSpecies, cancellationToken);
                                if (batchSpecies.Any())
                                {
                                    await speciesRepository.AddRangeAsync(batchSpecies, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchSpecies.Clear();
                                }

                                // Province işlemi
                                var province = await GetOrCreateProvinceWithBatch(dto, batchProvinces, cancellationToken);
                                if (batchProvinces.Any())
                                {
                                    await provinceRepository.AddRangeAsync(batchProvinces, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchProvinces.Clear();
                                }

                                // Location işlemi
                                var location = await GetOrCreateLocationWithBatch(dto, province?.Id, batchLocations, cancellationToken);
                                if (batchLocations.Any())
                                {
                                    await locationRepository.AddRangeAsync(batchLocations, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchLocations.Clear();
                                }

                                // Observer işlemi
                                var observer = await GetOrCreateObserverWithBatch(dto, batchObservers, cancellationToken);
                                if (batchObservers.Any())
                                {
                                    await observerRepository.AddRangeAsync(batchObservers, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    batchObservers.Clear();
                                }

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
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, $"Kayıt eklenirken hata oluştu: {ex.Message}");
                                continue;
                            }
                        }

                        if (batchObservations.Any())
                        {
                            await observationRepository.AddRangeAsync(batchObservations, cancellationToken);
                            await unitOfWork.SaveChangesAsync(cancellationToken);
                        }

                        totalProcessed += batch.Count;
                        logger.LogInformation($"Batch processed: {totalProcessed}/{importDtos.Count}");
                    }

                    // Duplicate kayıtların temizlenmesi
                    await CleanDuplicateObservations(cancellationToken);
                    // Transaction'ı commit et
                    await transaction.CommitAsync(cancellationToken);
                    result = ServiceResult.Success();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });

            return result ?? ServiceResult.Error("İşlem tamamlanamadı.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import işlemi sırasında hata oluştu");
            return ServiceResult.Error($"Import işlemi sırasında hata oluştu: {ex.Message}");
        }
    }
    private async Task<Authority> GetOrCreateAuthorityWithBatch(ImportCreateDto dto, List<Authority> batchList, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.AuthorityName)) return null;

        var authority = await authorityRepository.GetByNameAndYearAsync(dto.AuthorityName, dto.AuthorityYear, cancellationToken);
        if (authority == null)
        {
            authority = new Authority
            {
                Name = dto.AuthorityName,
                Year = dto.AuthorityYear
            };
            batchList.Add(authority);
        }
        return authority;
    }

    private async Task<Family> GetOrCreateFamilyWithBatch(ImportCreateDto dto, List<Family> batchList, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.FamilyName)) return null;

        var family = await familyRepository.GetByNameAsync(dto.FamilyName, cancellationToken);
        if (family == null)
        {
            family = new Family { Name = dto.FamilyName };
            batchList.Add(family);
        }
        return family;
    }

    private async Task<Genus> GetOrCreateGenusWithBatch(ImportCreateDto dto, int? familyId, List<Genus> batchList, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.GenusName)) return null;

        var genus = await genusRepository.GetByNameAsync(dto.GenusName, cancellationToken);
        if (genus == null)
        {
            genus = new Genus
            {
                Name = dto.GenusName,
                FamilyId = familyId
            };
            batchList.Add(genus);
        }
        return genus;
    }

    private async Task<Species> GetOrCreateSpeciesWithBatch(ImportCreateDto dto, int? genusId, int? authorityId, List<Species> batchList, CancellationToken cancellationToken)
    {
        var species = await speciesRepository.GetByName_Authority_GenusAsync(dto.SpeciesName, dto.AuthorityName, dto.GenusName, dto.AuthorityYear, cancellationToken);
        if (species == null)
        {
            species = new Species
            {
                GenusId = genusId,
                AuthorityId = authorityId,
                ScientificName = dto.ScientificName,
                Name = dto.SpeciesName,
                EUName = dto.EUName,
                FullName = dto.FullName,
                HesselbarthName = dto.HesselbarthName,
                TurkishName = dto.TurkishName,
                EnglishName = dto.EnglishName,
                TurkishNamesTrakel = dto.TurkishNamesTrakel,
                Trakel = dto.Trakel,
                KocakName = dto.KocakName
            };
            batchList.Add(species);
        }
        return species;
    }

    private async Task<Province> GetOrCreateProvinceWithBatch(ImportCreateDto dto, List<Province> batchList, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.ProvinceName) && !dto.ProvinceCode.HasValue) return null;

        var province = await provinceRepository.GetByNameOrCodeAsync(dto.ProvinceName, dto.ProvinceCode, cancellationToken);
        if (province == null)
        {
            province = new Province
            {
                Name = dto.ProvinceName,
                ProvinceCode = dto.ProvinceCode ?? 0
            };
            batchList.Add(province);
        }
        return province;
    }

    private async Task<Location> GetOrCreateLocationWithBatch(ImportCreateDto dto, int? provinceId, List<Location> batchList, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetByCoordinatesAsync(Math.Round(dto.Latitude, 6), Math.Round(dto.Longitude, 6), dto.SquareRef, cancellationToken);
        if (location == null)
        {
            location = new Location
            {
                ProvinceId = provinceId,
                SquareRef = dto.SquareRef,
                SquareLatitude = dto.SquareLatitude,
                SquareLongitude = dto.SquareLongitude,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                DecimalDegrees = dto.DecimalDegrees,
                DegreesMinutesSeconds = dto.DegreesMinutesSeconds,
                DecimalMinutes = dto.DecimalMinutes,
                UtmCoordinates = dto.UtmCoordinates,
                MgrsCoordinates = dto.MgrsCoordinates,
                Altitude1 = dto.Altitude1,
                Altitude2 = dto.Altitude2,
                UtmReference = dto.UtmReference,
                CoordinatePrecisionLevel = dto.CoordinatePrecisionLevel
            };
            batchList.Add(location);
        }
        return location;
    }

    private async Task<Observer> GetOrCreateObserverWithBatch(ImportCreateDto dto, List<Observer> batchList, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(dto.ObserverFullName) && string.IsNullOrEmpty(dto.ObserverName)) return null;

        var fullName = dto.ObserverFullName ?? dto.ObserverName;
        var observer = await observerRepository.GetByFullNameAsync(fullName, cancellationToken);
        if (observer == null)
        {
            observer = new Observer
            {
                Name = dto.ObserverName,
                Surname = dto.ObserverSurname,
                FullName = fullName
            };
            batchList.Add(observer);
        }
        return observer;
    }
    private async Task CleanDuplicateObservations(CancellationToken cancellationToken)
    {
        int batchSize = 1000;
        var duplicateIds = await observationRepository.GetAllDuplicateObservationIdsAsync(cancellationToken);
        if (duplicateIds.Any())
        {
            for (int i = 0; i < duplicateIds.Count; i += batchSize)
            {
                var batchIds = duplicateIds.Skip(i).Take(batchSize).ToList();
                unitOfWork.GetContext().ChangeTracker.Clear();
                await observationRepository.RemoveDuplicatesAsync(batchIds, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            logger.LogInformation($"Removed {duplicateIds.Count} duplicate observations");
        }
    }
    private Location NormalizeLocation(Location location)
    {
        // Koordinatları belirli bir hassasiyete yuvarlama
        location.Latitude = Math.Round(location.Latitude, 6);
        location.Longitude = Math.Round(location.Longitude, 6);
        return location;
    }
}