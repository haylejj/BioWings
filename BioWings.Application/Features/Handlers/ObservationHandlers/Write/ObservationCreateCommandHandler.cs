using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationCreateCommandHandler(IObservationRepository observationRepository, ISpeciesRepository speciesRepository, ISubspeciesRepository subspeciesRepository, ILocationRepository locationRepository, IProvinceRepository provinceRepository, IObserverRepository observerRepository, IAuthorityRepository authorityRepository, ISpeciesTypeRepository speciesTypeRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository, IUnitOfWork unitOfWork, ILogger<ObservationCreateCommandHandler> logger) : IRequestHandler<ObservationCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("ObservationCreateCommand request is null");
            return ServiceResult.Error("ObservationCreateCommand request is null");
        }
        // 1. Create or Get Authority
        var authority = await GetOrCreateAuthority(request, cancellationToken);
        if (!authority.IsSuccess)
            return ServiceResult.Error(authority.ErrorList);

        // 2. Create or Get Family
        var family = await GetOrCreateFamily(request, cancellationToken);
        if (!family.IsSuccess)
            return ServiceResult.Error(family.ErrorList);

        // 3. Create or Get Genus
        var genus = await GetOrCreateGenus(request, family.Data.Id, cancellationToken);
        if (!genus.IsSuccess)
            return ServiceResult.Error(genus.ErrorList);

        // 4. Create or Get SpeciesType
        //var speciesType = await GetOrCreateSpeciesType(request, cancellationToken);
        //if (!speciesType.IsSuccess)
        //    return ServiceResult<int>.Error(speciesType.ErrorList);

        // 5. Create or Get Species
        var species = await GetOrCreateSpecies(request, authority.Data.Id, genus.Data.Id, cancellationToken);
        if (!species.IsSuccess)
            return ServiceResult.Error(species.ErrorList);

        // 6. Create or Get Province
        //var province = await GetOrCreateProvince(request, cancellationToken);
        //if (!province.IsSuccess)
        //    return ServiceResult<int>.Error(province.ErrorList);

        // 7. Create or Get Location
        var location = await GetOrCreateLocation(request, request.ProvinceId, cancellationToken);
        if (!location.IsSuccess)
            return ServiceResult.Error(location.ErrorList);

        // 8. Create or Get Observer
        var observer = await GetOrCreateObserver(request, cancellationToken);
        if (!observer.IsSuccess)
            return ServiceResult.Error(observer.ErrorList);

        // 9. Create Observation
        var observation = new Observation
        {
            SpeciesId = species.Data.Id,
            LocationId = location.Data.Id,
            ObserverId = observer.Data.Id,
            Sex = request.Sex,
            ObservationDate = request.ObservationDate,
            LifeStage = request.LifeStage,
            NumberSeen = request.NumberSeen,
            Notes = request.Notes,
            Source = request.Source,
            LocationInfo = request.LocationInfo
        };

        await observationRepository.AddAsync(observation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Observation created successfully with ID: {ObservationId}", observation.Id);
        return ServiceResult.SuccessAsCreated("api/Observations/"+observation.Id);

    }
    private async Task<ServiceResult<Authority>> GetOrCreateAuthority(ObservationCreateCommand request, CancellationToken cancellationToken)
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
    private async Task<ServiceResult<Family>> GetOrCreateFamily(ObservationCreateCommand request, CancellationToken cancellationToken)
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
    private async Task<ServiceResult<Genus>> GetOrCreateGenus(ObservationCreateCommand request, int familyId, CancellationToken cancellationToken)
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
    private async Task<ServiceResult<Species>> GetOrCreateSpecies(ObservationCreateCommand request, int authorityId, int genusId, CancellationToken cancellationToken)
    {

        // Önce mevcut türü bulmaya çalış
        //Species species = null;

        // Farklı isimlere göre sırayla arama yap
        //if (!string.IsNullOrEmpty(request.ScientificName))
        //{
        //    species = await speciesRepository.GetByScientificNameAsync(request.ScientificName, cancellationToken);
        //}

        //if (species == null && !string.IsNullOrEmpty(request.KocakName))
        //{
        //    species = await speciesRepository.GetByKocakNameAsync(request.KocakName, cancellationToken);
        //}

        //if (species == null && !string.IsNullOrEmpty(request.HesselbarthName))
        //{
        //    species = await speciesRepository.GetByHesselbarthNameAsync(request.HesselbarthName, cancellationToken);
        //}
        var species = await speciesRepository.FirstOrDefaultAsync(s =>
        s.ScientificName == request.ScientificName &&
        s.GenusId == genusId &&
        s.AuthorityId == authorityId &&
        s.Name == request.Name &&
        s.EUName == request.EUName &&
        s.FullName == request.FullName &&
        s.TurkishName == request.TurkishName &&
        s.EnglishName == request.EnglishName &&
        s.TurkishNamesTrakel == request.TurkishNamesTrakel &&
        s.Trakel == request.Trakel &&
        s.KocakName == request.KocakName &&
        s.HesselbarthName == request.HesselbarthName,
        cancellationToken);


        if (species != null)
        {
            return ServiceResult<Species>.Success(species);
        }
        // Eğer tür bulunamadıysa ve gerekli bilgiler varsa yeni tür oluştur
        if (species == null && !string.IsNullOrEmpty(request.ScientificName))
        {
            species = new Species
            {
                AuthorityId = authorityId,
                GenusId = genusId,
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
    private async Task<ServiceResult<Location>> GetOrCreateLocation(ObservationCreateCommand request, int provinceId, CancellationToken cancellationToken)
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
    private async Task<Location> FindExistingLocation(ObservationCreateCommand request)
    {
        if (request.ProvinceId == default)
            return null;

        // Tam eşleşme kontrolü yap
        var exactMatch = await locationRepository.FirstOrDefaultAsync(l =>
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
            l.CoordinatePrecisionLevel == request.CoordinatePrecisionLevel);

        // Tam eşleşme varsa onu döndür
        if (exactMatch != null)
        {
            logger.LogInformation(
                "Found exact location match: ID={LocationId}, Province={ProvinceId}, SquareRef={SquareRef}",
                exactMatch.Id,
                exactMatch.ProvinceId,
                exactMatch.SquareRef);
            return exactMatch;
        }

        // Tam eşleşme yoksa null döndür, çağıran metod yeni kayıt oluşturacak
        return null;
    }
    private async Task<ServiceResult<Observer>> GetOrCreateObserver(ObservationCreateCommand request, CancellationToken cancellationToken)
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
    //private async Task<ServiceResult<Subspecies>> GetOrCreateSubspecies(ObservationCreateCommand request, int speciesId, CancellationToken cancellationToken)
    //{
    //    if (string.IsNullOrEmpty(request.SubspeciesName))
    //    {
    //        return ServiceResult<Subspecies>.Success(null); // Subspecies zorunlu değilse
    //    }

    //    var subspecies = await subspeciesRepository.GetByNameAndSpeciesIdAsync(
    //        request.SubspeciesName,
    //        speciesId,
    //        cancellationToken);

    //    if (subspecies == null)
    //    {
    //        subspecies = new Subspecies
    //        {
    //            Name = request.SubspeciesName,
    //            SpeciesId = speciesId
    //        };

    //        await subspeciesRepository.AddAsync(subspecies, cancellationToken);
    //        await unitOfWork.SaveChangesAsync(cancellationToken);

    //        logger.LogInformation(
    //            "New subspecies created: {SubspeciesName} for species ID: {SpeciesId}",
    //            request.SubspeciesName,
    //            speciesId);
    //    }

    //    return ServiceResult<Subspecies>.Success(subspecies);
    //}
}
