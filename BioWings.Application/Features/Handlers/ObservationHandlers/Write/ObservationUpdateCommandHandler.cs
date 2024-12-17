using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
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
        // Species güncelleme
        if (observation.Species != null)
        {
            // Yeni bir Species oluştur veya var olanı bul
            var species = await GetOrCreateSpecies(request, cancellationToken);
            if (!species.IsSuccess)
                return ServiceResult.Error(species.ErrorList);

            observation.SpeciesId = species.Data.Id;
        }

        if (observation.Location != null)
        {
            var location = await GetOrCreateLocation(request, request.ProvinceId, cancellationToken);
            if (!location.IsSuccess)
                return ServiceResult.Error(location.ErrorList);

            observation.LocationId = location.Data.Id;
        }

        if (observation.Observer != null)
        {
            var observer = await GetOrCreateObserver(request, cancellationToken);
            if (!observer.IsSuccess)
                return ServiceResult.Error(observer.ErrorList);

            observation.ObserverId = observer.Data.Id;
        }

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
    private async Task<ServiceResult<Family>> GetOrCreateFamily(ObservationUpdateCommand request, CancellationToken cancellationToken)
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
    private async Task<ServiceResult<Genus>> GetOrCreateGenus(ObservationUpdateCommand request, int familyId, CancellationToken cancellationToken)
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
    private async Task<ServiceResult<Species>> GetOrCreateSpecies(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        // Önce Authority, Family ve Genus oluştur/getir
        var authority = await GetOrCreateAuthority(request, cancellationToken);
        if (!authority.IsSuccess)
            return ServiceResult<Species>.Error(authority.ErrorList);

        var family = await GetOrCreateFamily(request, cancellationToken);
        if (!family.IsSuccess)
            return ServiceResult<Species>.Error(family.ErrorList);

        var genus = await GetOrCreateGenus(request, family.Data.Id, cancellationToken);
        if (!genus.IsSuccess)
            return ServiceResult<Species>.Error(genus.ErrorList);

        // Tam eşleşme kontrolü
        var existingSpecies = await speciesRepository.FirstOrDefaultAsync(s =>
            s.ScientificName == request.ScientificName &&
            s.GenusId == genus.Data.Id &&
            s.AuthorityId == authority.Data.Id &&
            s.Name == request.Name &&
            s.EUName == request.EUName &&
            s.FullName == request.FullName &&
            s.EnglishName == request.EnglishName &&
            s.TurkishName == request.TurkishName &&
            s.HesselbarthName == request.HesselbarthName &&
            s.KocakName == request.KocakName &&
            s.TurkishNamesTrakel == request.TurkishNamesTrakel &&
            s.Trakel == request.Trakel,
            cancellationToken);

        // Eğer tam eşleşme varsa, mevcut kaydı kullan
        if (existingSpecies != null)
        {
            return ServiceResult<Species>.Success(existingSpecies);
        }

        // Tam eşleşme yoksa, yeni kayıt oluştur
        var newSpecies = new Species
        {
            AuthorityId = authority.Data.Id,
            GenusId = genus.Data.Id,
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
    private async Task<ServiceResult<Location>> GetOrCreateLocation(ObservationUpdateCommand request, int provinceId, CancellationToken cancellationToken)
    {
        // Tam eşleşme kontrolü
        var existingLocation = await locationRepository.FirstOrDefaultAsync(l =>
            l.ProvinceId == provinceId &&
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
            l.CoordinatePrecisionLevel == request.CoordinatePrecisionLevel);

        if (existingLocation != null)
        {
            return ServiceResult<Location>.Success(existingLocation);
        }

        // Yeni kayıt oluştur
        var newLocation = new Location
        {
            ProvinceId = provinceId,
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
    private async Task<ServiceResult<Observer>> GetOrCreateObserver(ObservationUpdateCommand request, CancellationToken cancellationToken)
    {
        var observer = await observerRepository.GetByNameAndSurnameAsync(
            request.ObserverName,
            request.Surname,
            cancellationToken);

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
}
