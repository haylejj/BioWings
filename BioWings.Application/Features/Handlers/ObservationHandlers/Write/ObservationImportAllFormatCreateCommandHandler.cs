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

            const int batchSize = 1500;
            var totalProcessed = 0;

            await unitOfWork.GetContext().Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = await unitOfWork.GetContext().Database.BeginTransactionAsync(cancellationToken);
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
                        // Authority işlemi
                        var authority = !string.IsNullOrEmpty(group.Key.AuthorityName)
                            ? await GetOrCreateAuthorityAsync(group.Key.AuthorityName, group.Key.AuthorityYear, cancellationToken)
                            : null;

                        // Family işlemi
                        var family = !string.IsNullOrEmpty(group.Key.FamilyName)
                            ? await GetOrCreateFamilyAsync(group.Key.FamilyName, cancellationToken)
                            : null;

                        // Genus işlemi
                        var genus = !string.IsNullOrEmpty(group.Key.GenusName)
                            ? await GetOrCreateGenusAsync(group.Key.GenusName, family?.Id, cancellationToken)
                            : null;

                        // Province işlemi
                        var province = (!string.IsNullOrEmpty(group.Key.ProvinceName) || group.Key.ProvinceCode.HasValue)
                            ? await GetOrCreateProvinceAsync(group.Key.ProvinceName, group.Key.ProvinceCode, cancellationToken)
                            : null;

                        // Observer işlemi
                        var observer = !string.IsNullOrEmpty(group.Key.ObserverFullName)
                            ? await GetOrCreateObserverAsync(group.Key.ObserverFullName, cancellationToken)
                            : null;

                        var batchObservations = new List<Observation>();

                        foreach (var dto in group)
                        {
                            try
                            {
                                // Species işlemi
                                var species = await GetOrCreateSpeciesAsync(dto, genus?.Id, authority?.Id, cancellationToken);

                                // Location işlemi
                                var location = await GetOrCreateLocationAsync(dto, province?.Id, cancellationToken);

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

                                if (batchObservations.Count >= batchSize)
                                {
                                    await observationRepository.AddRangeAsync(batchObservations, cancellationToken);
                                    await unitOfWork.SaveChangesAsync(cancellationToken);
                                    totalProcessed += batchObservations.Count;
                                    logger.LogInformation($"Batch processed: {totalProcessed}/{importDtos.Count}");
                                    batchObservations.Clear();
                                    unitOfWork.GetContext().ChangeTracker.Clear();
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, $"Kayıt eklenirken hata oluştu: {ex.Message}");
                                continue;
                            }
                        }

                        // Kalan observations varsa onları da ekle
                        if (batchObservations.Any())
                        {
                            await observationRepository.AddRangeAsync(batchObservations, cancellationToken);
                            await unitOfWork.SaveChangesAsync(cancellationToken);
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
        var authority = await authorityRepository.GetByNameAndYearAsync(name, year, cancellationToken);
        if (authority == null)
        {
            authority = new Authority { Name = name, Year = year };
            await authorityRepository.AddAsync(authority, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return authority;
    }

    private async Task<Family> GetOrCreateFamilyAsync(string name, CancellationToken cancellationToken)
    {
        var family = await familyRepository.GetByNameAsync(name, cancellationToken);
        if (family == null)
        {
            family = new Family { Name = name };
            await familyRepository.AddAsync(family, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return family;
    }

    private async Task<Genus> GetOrCreateGenusAsync(string name, int? familyId, CancellationToken cancellationToken)
    {
        var genus = await genusRepository.GetByNameAsync(name, cancellationToken);
        if (genus == null)
        {
            genus = new Genus { Name = name, FamilyId = familyId };
            await genusRepository.AddAsync(genus, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return genus;
    }

    private async Task<Species> GetOrCreateSpeciesAsync(ImportCreateDto dto, int? genusId, int? authorityId, CancellationToken cancellationToken)
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
            await speciesRepository.AddAsync(species, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return species;
    }

    private async Task<Location> GetOrCreateLocationAsync(ImportCreateDto dto, int? provinceId, CancellationToken cancellationToken)
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
                Latitude = Math.Round(dto.Latitude, 6),
                Longitude = Math.Round(dto.Longitude, 6),
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
            await locationRepository.AddAsync(location, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return location;
    }
    private async Task<Province> GetOrCreateProvinceAsync(string name, int? code, CancellationToken cancellationToken)
    {
        var province = await provinceRepository.GetByNameOrCodeAsync(name, code, cancellationToken);
        if (province == null)
        {
            province = new Province
            {
                Name = name,
                ProvinceCode = code ?? 0
            };
            await provinceRepository.AddAsync(province, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return province;
    }

    private async Task<Observer> GetOrCreateObserverAsync(string fullName, CancellationToken cancellationToken)
    {
        var observer = await observerRepository.GetByFullNameAsync(fullName, cancellationToken);
        if (observer == null)
        {
            // İsim ve soyisim ayırma işlemi
            var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var surname = nameParts.Length > 1 ? nameParts[^1] : string.Empty;
            var name = nameParts.Length > 1 ? string.Join(" ", nameParts.Take(nameParts.Length - 1)) : fullName;

            observer = new Observer
            {
                Name = name,
                Surname = surname,
                FullName = fullName
            };
            await observerRepository.AddAsync(observer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return observer;
    }

    private async Task CleanDuplicateObservations(CancellationToken cancellationToken)
    {
        const int batchSize = 1500;
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
}