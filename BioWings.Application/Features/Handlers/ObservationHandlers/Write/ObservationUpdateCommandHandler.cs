using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationUpdateCommandHandler(IObservationRepository observationRepository, IFamilyRepository familyRepository, ILocationRepository locationRepository, IObserverRepository observerRepository, IGenusRepository genusRepository, IAuthorityRepository authorityRepository, ISpeciesRepository speciesRepository, IUnitOfWork unitOfWork, ILogger<ObservationUpdateCommandHandler> logger) : IRequestHandler<ObservationUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        var observation = await observationRepository.GetByIdWithAllNavigationsAsync(request.Id);
        if (observation == null)
        {
            logger.LogError($"Observation with id {request.Id} not found");
            return ServiceResult.Error($"Observation with id {request.Id} not found", System.Net.HttpStatusCode.NotFound);
        }

        var species = await GetOrCreateSpecies(request, cancellationToken);
        if (!species.IsSuccess)
            return ServiceResult.Error(species.ErrorList);

        observation.SpeciesId = species.Data.Id;


        var location = await GetOrCreateLocation(request, cancellationToken);
        if (!location.IsSuccess)
            return ServiceResult.Error(location.ErrorList);

        observation.LocationId = location.Data?.Id;

        var observer = await GetOrCreateObserver(request, cancellationToken);
        if (!observer.IsSuccess)
            return ServiceResult.Error(observer.ErrorList);

        observation.ObserverId = observer.Data?.Id;

        // Ana observation bilgilerini güncelleme
        observation.Sex = request.Sex;
        observation.ObservationDate = request.ObservationDate;
        observation.LifeStage = request.LifeStage;
        observation.NumberSeen = request.NumberSeen;
        observation.Notes = request.Notes;
        observation.Source = request.Source;
        observation.LocationInfo = request.LocationInfo;

        observationRepository.Update(observation);



        logger.LogInformation($"Observation with id {request.Id} updated successfully");
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var unusedSpecies = await speciesRepository.GetUnusedSpeciesRecord().ToListAsync();
        if (unusedSpecies.Any())
            speciesRepository.RemoveRange(unusedSpecies);

        var unsusedLocations = await locationRepository.GetUnusedLocationRecord().ToListAsync();
        if (unsusedLocations.Any())
            locationRepository.RemoveRange(unsusedLocations);
        logger.LogInformation("Successfully deleted {Count} unused species records", unusedSpecies.Count);
        logger.LogInformation("Successfully deleted {Count} unused location records", unsusedLocations.Count);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
    private async Task<ServiceResult<Authority>> GetOrCreateAuthority(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.AuthorityName))
            {
                return ServiceResult<Authority>.Success(null);
            }

            var authority = await authorityRepository.GetByNameAndYearAsync(
                request.AuthorityName,
                request.Year,
                cancellationToken);

            if (authority != null)
            {
                logger.LogInformation("Found existing authority with id: {AuthorityId}", authority.Id);
                return ServiceResult<Authority>.Success(authority);
            }

            authority = new Authority
            {
                Name = request.AuthorityName,
                Year = request.Year ?? 0 // 0 yerine bir default year belirlenebilir
            };

            await authorityRepository.AddAsync(authority, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new authority with id: {AuthorityId}", authority.Id);

            return ServiceResult<Authority>.Success(authority);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating/getting authority");
            return ServiceResult<Authority>.Error($"Error in authority operation: {ex.Message}");
        }
    }
    private async Task<ServiceResult<Family>> GetOrCreateFamily(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.FamilyName))
            {
                return ServiceResult<Family>.Success(null);
            }

            var family = await familyRepository.GetByNameAsync(request.FamilyName, cancellationToken);
            if (family != null)
            {
                logger.LogInformation("Found existing family with id: {FamilyId}", family.Id);
                return ServiceResult<Family>.Success(family);
            }

            family = new Family { Name = request.FamilyName };
            await familyRepository.AddAsync(family, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new family with id: {FamilyId}", family.Id);

            return ServiceResult<Family>.Success(family);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating/getting family");
            return ServiceResult<Family>.Error($"Error in family operation: {ex.Message}");
        }
    }

    private async Task<ServiceResult<Genus>> GetOrCreateGenus(ObservationUpdateCommand request, int? familyId, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.GenusName))
            {
                return ServiceResult<Genus>.Success(null);
            }

            var genus = await genusRepository.GetByNameAndFamilyIdAsync(request.GenusName, familyId, cancellationToken);
            if (genus != null)
            {
                logger.LogInformation("Found existing genus with id: {GenusId}", genus.Id);
                return ServiceResult<Genus>.Success(genus);
            }

            genus = new Genus
            {
                Name = request.GenusName,
                FamilyId = familyId
            };
            await genusRepository.AddAsync(genus, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new genus with id: {GenusId}", genus.Id);

            return ServiceResult<Genus>.Success(genus);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating/getting genus");
            return ServiceResult<Genus>.Error($"Error in genus operation: {ex.Message}");
        }
    }
    private async Task<ServiceResult<Species>> GetOrCreateSpecies(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        // Önce Authority, Family ve Genus oluştur/getir
        var authority = await GetOrCreateAuthority(request, cancellationToken);
        if (!authority.IsSuccess)
            return ServiceResult<Species>.Error(authority.ErrorList);

        var family = await GetOrCreateFamily(request, cancellationToken);
        if (!family.IsSuccess)
            return ServiceResult<Species>.Error(family.ErrorList);

        var genus = await GetOrCreateGenus(request, family.Data?.Id, cancellationToken);
        if (!genus.IsSuccess)
            return ServiceResult<Species>.Error(genus.ErrorList);
        var genusId = genus.Data?.Id;
        var authorityId = authority.Data?.Id;

        var existingSpecies = await speciesRepository.FirstOrDefaultAsync(s =>
            s.ScientificName == request.ScientificName &&
            // null olmayacaksa Id karşılaştırmayı yap, aksi durumda false döndürülsün
            genusId.HasValue &&s.GenusId == genusId.Value &&
            authorityId.HasValue &&s.AuthorityId == authorityId.Value &&
            s.Name == request.Name &&
            s.EUName == request.EUName &&
            s.FullName == request.FullName &&
            s.EnglishName == request.EnglishName &&
            s.TurkishName == request.TurkishName &&
            s.HesselbarthName == request.HesselbarthName &&
            s.KocakName == request.KocakName &&
            s.TurkishNamesTrakel == request.TurkishNamesTrakel &&
            s.Trakel == request.Trakel,
            cancellationToken
        );

        // Eğer tam eşleşme varsa, mevcut kaydı kullan
        if (existingSpecies != null)
        {
            return ServiceResult<Species>.Success(existingSpecies);
        }

        // Tam eşleşme yoksa, yeni kayıt oluştur
        var newSpecies = new Species
        {
            AuthorityId = authorityId,
            GenusId = genusId,
            ScientificName = request.ScientificName,
            Name = request.Name,
            EUName = request.EUName,
            FullName = request.FullName,
            EnglishName = request.EnglishName,
            TurkishName = request.TurkishName,
            HesselbarthName = request.HesselbarthName,
            KocakName = request.KocakName,
            TurkishNamesTrakel = request.TurkishNamesTrakel,
            Trakel = request.Trakel
        };

        await speciesRepository.AddAsync(newSpecies, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Species>.Success(newSpecies);
    }
    private async Task<ServiceResult<Location>> GetOrCreateLocation(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        // Mevcut observation'ı al
        var observation = await observationRepository.GetByIdWithAllNavigationsAsync(request.Id);
        if (observation == null)
        {
            return ServiceResult<Location>.Error("Observation not found");
        }

        // Eğer location değişmemişse mevcut location'ı döndür
        if (observation.Location != null &&
            (observation.Location.ProvinceId ?? 0) == (request.ProvinceId ?? 0) &&
            (observation.Location.SquareRef ?? "") == (request.SquareRef ?? "") &&
            observation.Location.Latitude == request.Latitude &&
            observation.Location.Longitude == request.Longitude &&
            observation.Location.SquareLatitude == request.SquareLatitude &&
            observation.Location.SquareLongitude == request.SquareLongitude &&
            (observation.Location.DecimalDegrees ?? "") == (request.DecimalDegrees ?? "") &&
            (observation.Location.DegreesMinutesSeconds ?? "") == (request.DegreesMinutesSeconds ?? "") &&
            (observation.Location.DecimalMinutes ?? "") == (request.DecimalMinutes ?? "") &&
            (observation.Location.UtmCoordinates ?? "") == (request.UtmCoordinates ?? "") &&
            (observation.Location.MgrsCoordinates ?? "") == (request.MgrsCoordinates ?? "") &&
            observation.Location.Altitude1 == request.Altitude1 &&
            observation.Location.Altitude2 == request.Altitude2 &&
            (observation.Location.UtmReference ?? "") == (request.UtmReference ?? "") &&
            observation.Location.CoordinatePrecisionLevel == request.CoordinatePrecisionLevel)
        {
            return ServiceResult<Location>.Success(observation.Location);
        }

        // Aynı bilgilere sahip başka bir location var mı diye kontrol et
        var existingLocation = await locationRepository.FirstOrDefaultAsync(l =>
            l.ProvinceId == request.ProvinceId &&
            l.SquareRef == request.SquareRef &&
            l.Latitude == request.Latitude &&
            l.Longitude == request.Longitude &&
            l.SquareLatitude == request.SquareLatitude &&
            l.SquareLongitude == request.SquareLongitude &&
            l.DecimalDegrees == request.DecimalDegrees &&
            l.DegreesMinutesSeconds == request.DegreesMinutesSeconds &&
            l.DecimalMinutes == request.DecimalMinutes &&
            l.UtmCoordinates == request.UtmCoordinates &&
            l.MgrsCoordinates == request.MgrsCoordinates &&
            l.Altitude1 == request.Altitude1 &&
            l.Altitude2 == request.Altitude2 &&
            l.UtmReference == request.UtmReference &&
            l.CoordinatePrecisionLevel == request.CoordinatePrecisionLevel,
            cancellationToken);

        if (existingLocation != null)
        {
            return ServiceResult<Location>.Success(existingLocation);
        }

        try
        {
            // Yeni location oluştur
            var newLocation = new Location
            {
                ProvinceId = request.ProvinceId,
                SquareRef = request.SquareRef,
                SquareLatitude = request.SquareLatitude,
                SquareLongitude = request.SquareLongitude,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                DecimalDegrees = request.DecimalDegrees,
                DegreesMinutesSeconds = request.DegreesMinutesSeconds,
                DecimalMinutes = request.DecimalMinutes,
                UtmCoordinates = request.UtmCoordinates,
                MgrsCoordinates = request.MgrsCoordinates,
                Altitude1 = request.Altitude1,
                Altitude2 = request.Altitude2,
                UtmReference = request.UtmReference,
                CoordinatePrecisionLevel = request.CoordinatePrecisionLevel
            };

            await locationRepository.AddAsync(newLocation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult<Location>.Success(newLocation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating new location");
            return ServiceResult<Location>.Error($"Error creating location: {ex.Message}");
        }
    }
    private async Task<ServiceResult<Observer>> GetOrCreateObserver(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Eğer observer name boşsa null dönebiliriz
            if (string.IsNullOrEmpty(request.ObserverName))
            {
                return ServiceResult<Observer>.Success(null);
            }

            var observer = await observerRepository.GetByNameAndSurnameAsync(
                request.ObserverName,
                request.Surname,
                cancellationToken);

            if (observer != null)
            {
                logger.LogInformation("Found existing observer with id: {ObserverId}", observer.Id);
                return ServiceResult<Observer>.Success(observer);
            }

            observer = new Observer
            {
                Name = request.ObserverName,
                Surname = request.Surname,
                FullName = string.IsNullOrEmpty(request.ObserverFullName)
                    ? $"{request.ObserverName} {request.Surname}".Trim()
                    : request.ObserverFullName
            };

            await observerRepository.AddAsync(observer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new observer with id: {ObserverId}", observer.Id);

            return ServiceResult<Observer>.Success(observer);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating/getting observer");
            return ServiceResult<Observer>.Error($"Error in observer operation: {ex.Message}");
        }
    }
}
