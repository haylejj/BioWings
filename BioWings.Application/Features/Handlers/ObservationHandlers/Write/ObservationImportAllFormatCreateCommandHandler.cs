using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationImportAllFormatCreateCommandHandler(ILogger<ObservationImportAllFormatCreateCommandHandler> logger, IUnitOfWork unitOfWork, IExcelImportService excelImportService, IObservationRepository observationRepository, ISubspeciesRepository subspeciesRepository, ISpeciesRepository speciesRepository, ILocationRepository locationRepository, IProvinceRepository provinceRepository, IObserverRepository observerRepository, IAuthorityRepository authorityRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository) : IRequestHandler<ObservationImportAllFormatCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationImportAllFormatCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var importDtos = excelImportService.ImportFromExcel(request.File);
            logger.LogInformation($"Excel'den {importDtos.Count} satır okundu.");
            var context = unitOfWork.GetContext();
            const int batchSize = 1500;
            var totalProcessed = 0;

            await unitOfWork.GetContext().Database.CreateExecutionStrategy().ExecuteAsync(async () =>
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
                            var species = await speciesRepository.GetOrCreateSpeciesAsync(dto, genus?.Id, authority?.Id, cancellationToken);

                            // Location işlemi
                            var location = await locationRepository.GetOrCreateLocationAsync(dto, province?.Id, cancellationToken);

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