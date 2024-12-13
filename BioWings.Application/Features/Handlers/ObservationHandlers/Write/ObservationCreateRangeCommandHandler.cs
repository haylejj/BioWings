using BioWings.Application.DTOs.ObservationDtos;
using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationCreateRangeCommandHandler(IObservationRepository observationRepository, ISubspeciesRepository subspeciesRepository, ISpeciesRepository speciesRepository, ILocationRepository locationRepository, IProvinceRepository provinceRepository, IObserverRepository observerRepository, IAuthorityRepository authorityRepository, ISpeciesTypeRepository speciesTypeRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository, IUnitOfWork unitOfWork, ILogger<ObservationCreateRangeCommandHandler> logger) : IRequestHandler<ObservationCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationCreateRangeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting bulk creation of {Count} observations", request.ObservationCreateDtos.Count);

        var successCount = 0;
        var errors = new List<string>();
        var duplicateCount = 0;

        foreach (var observationCommand in request.ObservationCreateDtos)
        {
            // Authority
            var authorityResult = await GetOrCreateAuthority(observationCommand, cancellationToken);
            if (!authorityResult.IsSuccess)
            {
                errors.Add($"Error with Authority for observation {successCount + 1}: {authorityResult.ErrorList}");
                continue;
            }

            // Family
            var familyResult = await GetOrCreateFamily(observationCommand, cancellationToken);
            if (!familyResult.IsSuccess)
            {
                errors.Add($"Error with Family for observation {successCount + 1}: {familyResult.ErrorList}");
                continue;
            }

            // Genus
            var genusResult = await GetOrCreateGenus(observationCommand, familyResult.Data.Id, cancellationToken);
            if (!genusResult.IsSuccess)
            {
                errors.Add($"Error with Genus for observation {successCount + 1}: {genusResult.ErrorList}");
                continue;
            }

            // SpeciesType
            var speciesTypeResult = await GetOrCreateSpeciesType(observationCommand, cancellationToken);
            if (!speciesTypeResult.IsSuccess)
            {
                errors.Add($"Error with SpeciesType for observation {successCount + 1}: {speciesTypeResult.ErrorList}");
                continue;
            }

            // Species
            var speciesResult = await GetOrCreateSpecies(
                observationCommand,
                authorityResult.Data.Id,
                genusResult.Data.Id,
                speciesTypeResult.Data.Id,
                cancellationToken);
            if (!speciesResult.IsSuccess)
            {
                errors.Add($"Error with Species for observation {successCount + 1}: {speciesResult.ErrorList}");
                continue;
            }

            // Province
            var provinceResult = await GetOrCreateProvince(observationCommand, cancellationToken);
            if (!provinceResult.IsSuccess)
            {
                errors.Add($"Error with Province for observation {successCount + 1}: {provinceResult.ErrorList}");
                continue;
            }

            // Location
            var locationResult = await GetOrCreateLocation(observationCommand, provinceResult.Data.Id, cancellationToken);
            if (!locationResult.IsSuccess)
            {
                errors.Add($"Error with Location for observation {successCount + 1}: {locationResult.ErrorList}");
                continue;
            }

            // Observer
            var observerResult = await GetOrCreateObserver(observationCommand, cancellationToken);
            if (!observerResult.IsSuccess)
            {
                errors.Add($"Error with Observer for observation {successCount + 1}: {observerResult.ErrorList}");
                continue;
            }

            // Subspecies
            if (speciesResult.IsSuccess)
            {
                var subspeciesResult = await GetOrCreateSubspecies(
                    observationCommand,
                    speciesResult.Data.Id,
                    cancellationToken);

                if (!subspeciesResult.IsSuccess)
                {
                    errors.Add($"Error with Subspecies for observation {successCount + 1}: {subspeciesResult.ErrorList}");
                    continue;
                }
            }

            // Check for duplicate observation
            var existingObservation = await observationRepository.FirstOrDefaultAsync(o =>
                o.SpeciesId == speciesResult.Data.Id &&
                o.LocationId == locationResult.Data.Id &&
                o.ObserverId == observerResult.Data.Id &&
                o.Sex == observationCommand.Sex &&
                o.ObservationDate == observationCommand.ObservationDate &&
                o.LifeStage == observationCommand.LifeStage &&
                o.NumberSeen == observationCommand.NumberSeen &&
                o.Notes == observationCommand.Notes &&
                o.Source == observationCommand.Source &&
                o.LocationInfo == observationCommand.LocationInfo,
                cancellationToken);

            if (existingObservation == null)
            {
                // Create new Observation
                var observation = new Observation
                {
                    SpeciesId = speciesResult.Data.Id,
                    LocationId = locationResult.Data.Id,
                    ObserverId = observerResult.Data.Id,
                    Sex = observationCommand.Sex,
                    ObservationDate = observationCommand.ObservationDate,
                    LifeStage = observationCommand.LifeStage,
                    NumberSeen = observationCommand.NumberSeen,
                    Notes = observationCommand.Notes,
                    Source = observationCommand.Source,
                    LocationInfo = observationCommand.LocationInfo
                };

                await observationRepository.AddAsync(observation, cancellationToken);
                successCount++;

                // Save changes every 100 records
                if (successCount % 100 == 0)
                {
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    logger.LogInformation("Saved batch of 100 observations. Total success count: {SuccessCount}", successCount);
                }
            }
            else
            {
                duplicateCount++;
                logger.LogInformation(
                    "Duplicate observation found and skipped - Species:{SpeciesId}, Location:{LocationId}, Observer:{ObserverId}, Date:{Date}",
                    speciesResult.Data.Id,
                    locationResult.Data.Id,
                    observerResult.Data.Id,
                    observationCommand.ObservationDate);
            }
        }

        // Save any remaining records
        if (successCount % 100 != 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation(
            "Import completed. Created: {SuccessCount}, Duplicates skipped: {DuplicateCount}, Errors: {ErrorCount}",
            successCount,
            duplicateCount,
            errors.Count);

        return errors.Any()
            ? ServiceResult.Error($"Completed with {errors.Count} errors. First error: {errors.First()}")
            : ServiceResult.SuccessAsCreated("api/Observations/Range");
    }
    private async Task<ServiceResult<Authority>> GetOrCreateAuthority(ObservationCreateDto request, CancellationToken cancellationToken)
    {

        var authority = await authorityRepository.GetByNameAndYearAsync(request.AuthorityName, request.Year, cancellationToken);
        if (authority == null && !string.IsNullOrEmpty(request.AuthorityName))
        {
            authority = new Authority
            {
                Name = request.AuthorityName,
                Year = request.Year
            };
            await authorityRepository.AddAsync(authority, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Authority created that has id : {0}", authority.Id);
        }
        return authority != null ?
            ServiceResult<Authority>.Success(authority) :
            ServiceResult<Authority>.Error("Failed to create authority");
    }
    private async Task<ServiceResult<Family>> GetOrCreateFamily(ObservationCreateDto request, CancellationToken cancellationToken)
    {

        var family = await familyRepository.GetByNameAsync(request.FamilyName, cancellationToken);
        if (family == null && !string.IsNullOrEmpty(request.FamilyName))
        {
            family = new Family { Name = request.FamilyName };
            await familyRepository.AddAsync(family, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return family != null ?
            ServiceResult<Family>.Success(family) :
            ServiceResult<Family>.Error("Failed to create family");

    }
    private async Task<ServiceResult<Genus>> GetOrCreateGenus(ObservationCreateDto request, int familyId, CancellationToken cancellationToken)
    {

        var genus = await genusRepository.GetByNameAndFamilyIdAsync(request.GenusName, familyId, cancellationToken);
        if (genus == null && !string.IsNullOrEmpty(request.GenusName))
        {
            genus = new Genus
            {
                Name = request.GenusName,
                FamilyId = familyId
            };
            await genusRepository.AddAsync(genus, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return genus != null ?
            ServiceResult<Genus>.Success(genus) :
            ServiceResult<Genus>.Error("Failed to create genus");
    }
    private async Task<ServiceResult<SpeciesType>> GetOrCreateSpeciesType(ObservationCreateDto request, CancellationToken cancellationToken)
    {

        var speciesType = await speciesTypeRepository.GetByNameAndDescriptionAsync(request.SpeciesTypeName, request.SpeciesTypeDescription, cancellationToken);
        if (speciesType == null && !string.IsNullOrEmpty(request.SpeciesTypeName))
        {
            speciesType = new SpeciesType
            {
                Name = request.SpeciesTypeName,
                Description = request.SpeciesTypeDescription
            };
            await speciesTypeRepository.AddAsync(speciesType, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("New species type created: {SpeciesTypeName} with description: {Description}", request.SpeciesTypeName, request.SpeciesTypeDescription);

        }
        return speciesType != null ?
            ServiceResult<SpeciesType>.Success(speciesType) :
            ServiceResult<SpeciesType>.Error("Failed to create species type");

    }
    private async Task<ServiceResult<Species>> GetOrCreateSpecies(ObservationCreateDto request, int authorityId, int genusId, int speciesTypeId, CancellationToken cancellationToken)
    {

        // Önce mevcut türü bulmaya çalış
        Species species = null;

        // Farklı isimlere göre sırayla arama yap
        if (!string.IsNullOrEmpty(request.ScientificName))
        {
            species = await speciesRepository.GetByScientificNameAsync(request.ScientificName, cancellationToken);
        }

        if (species == null && !string.IsNullOrEmpty(request.KocakName))
        {
            species = await speciesRepository.GetByKocakNameAsync(request.KocakName, cancellationToken);
        }

        if (species == null && !string.IsNullOrEmpty(request.HesselbarthName))
        {
            species = await speciesRepository.GetByHesselbarthNameAsync(request.HesselbarthName, cancellationToken);
        }

        // Eğer tür bulunamadıysa ve gerekli bilgiler varsa yeni tür oluştur
        if (species == null && !string.IsNullOrEmpty(request.ScientificName))
        {
            species = new Species
            {
                AuthorityId = authorityId,
                GenusId = genusId,
                SpeciesTypeId = speciesTypeId,
                ScientificName = request.ScientificName,
                Name = request.Name,
                EUName = request.EUName,
                FullName = request.FullName,
                TurkishName = request.TurkishName,
                EnglishName = request.EnglishName,
                TurkishNamesTrakel = request.TurkishNamesTrakel,
                Trakel = request.Trakel,
                KocakName = request.KocakName,
                HesselbarthName = request.HesselbarthName
            };

            await speciesRepository.AddAsync(species, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("New species created: {ScientificName}", request.ScientificName);
        }
        return species == null
            ? ServiceResult<Species>.Error("Species could not be found or created")
            : ServiceResult<Species>.Success(species);
    }
    private async Task<ServiceResult<Province>> GetOrCreateProvince(ObservationCreateDto request, CancellationToken cancellationToken)
    {

        var province = await provinceRepository.GetByNameAsync(request.ProvinceName, cancellationToken);
        if (province == null && !string.IsNullOrEmpty(request.ProvinceName))
        {
            province = new Province
            {
                Name = request.ProvinceName,
                ProvinceCode = request.ProvinceCode
            };
            await provinceRepository.AddAsync(province, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return province != null ?
            ServiceResult<Province>.Success(province) :
            ServiceResult<Province>.Error("Failed to create province");

    }
    private async Task<ServiceResult<Location>> GetOrCreateLocation(ObservationCreateDto request, int provinceId, CancellationToken cancellationToken)
    {
        // Mevcut lokasyonu bul
        var existingLocation = await FindExistingLocation(request);

        if (existingLocation == null)
        {
            // Yeni lokasyon oluştur
            var location = new Location
            {
                ProvinceId = provinceId,
                SquareRef = request.SquareRef,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Altitude1 = request.Altitude1,
                Altitude2 = request.Altitude2,
                UtmReference = request.UtmReference,
                Description = request.Description,
                DecimalMinutes = request.DecimalMinutes,
                DecimalDegrees = request.DecimalDegrees,
                DegreesMinutesSeconds = request.DegreesMinutesSeconds,
                CoordinatePrecisionLevel = request.CoordinatePrecisionLevel,
                MgrsCoordinates = request.MgrsCoordinates,
                SquareLatitude = request.SquareLatitude,
                SquareLongitude = request.SquareLongitude,
                UtmCoordinates = request.UtmCoordinates
            };

            await locationRepository.AddAsync(location, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Location>.Success(location);
        }

        return ServiceResult<Location>.Success(existingLocation);

    }
    private async Task<Location> FindExistingLocation(ObservationCreateDto request)
    {
        // 1. Önce SquareRef ile kontrol
        if (!string.IsNullOrEmpty(request.SquareRef))
        {
            var locationByRef = await locationRepository.FirstOrDefaultAsync(l =>
                l.SquareRef == request.SquareRef);
            if (locationByRef != null) return locationByRef;
        }

        // 2. Tam koordinat eşleşmesi
        var locationByExactCoords = await locationRepository.FirstOrDefaultAsync(l =>
            l.Latitude == request.Latitude &&
            l.Longitude == request.Longitude);
        if (locationByExactCoords != null) return locationByExactCoords;

        // 3. Square koordinatları ile kontrol
        var locationBySquare = await locationRepository.FirstOrDefaultAsync(l =>
            l.SquareLatitude == request.SquareLatitude &&
            l.SquareLongitude == request.SquareLongitude);
        if (locationBySquare != null) return locationBySquare;

        // 4. Yakın koordinatlar için toleranslı kontrol
        const decimal TOLERANCE = 0.0001m;
        var locationByTolerance = await locationRepository.FirstOrDefaultAsync(l =>
            Math.Abs(l.Latitude - request.Latitude) <= TOLERANCE &&
            Math.Abs(l.Longitude - request.Longitude) <= TOLERANCE);

        return locationByTolerance;
    }
    private async Task<ServiceResult<Observer>> GetOrCreateObserver(ObservationCreateDto request, CancellationToken cancellationToken)
    {

        var observer = await observerRepository.GetByNameAndSurnameAsync(request.ObserverName, request.Surname, cancellationToken);
        if (observer == null && !string.IsNullOrEmpty(request.ObserverName))
        {
            observer = new Observer
            {
                Name = request.ObserverName,
                Surname = request.Surname,
                FullName = request.ObserverFullName
            };
            await observerRepository.AddAsync(observer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return observer != null ?
            ServiceResult<Observer>.Success(observer) :
            ServiceResult<Observer>.Error("Failed to create observer");

    }
    private async Task<ServiceResult<Subspecies>> GetOrCreateSubspecies(ObservationCreateDto request, int speciesId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SubspeciesName))
        {
            return ServiceResult<Subspecies>.Success(null); // Subspecies zorunlu değilse
        }

        var subspecies = await subspeciesRepository.GetByNameAndSpeciesIdAsync(
            request.SubspeciesName,
            speciesId,
            cancellationToken);

        if (subspecies == null)
        {
            subspecies = new Subspecies
            {
                Name = request.SubspeciesName,
                SpeciesId = speciesId
            };

            await subspeciesRepository.AddAsync(subspecies, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "New subspecies created: {SubspeciesName} for species ID: {SpeciesId}",
                request.SubspeciesName,
                speciesId);
        }

        return ServiceResult<Subspecies>.Success(subspecies);
    }
}
