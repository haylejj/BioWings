using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationCreateCommandHandler(IObservationRepository observationRepository,ISpeciesRepository speciesRepository,ILocationRepository locationRepository,IProvinceRepository provinceRepository,IObserverRepository observerRepository,IAuthorityRepository authorityRepository,ISpeciesTypeRepository speciesTypeRepository,IFamilyRepository familyRepository,IGenusRepository genusRepository, IUnitOfWork unitOfWork, ILogger<ObservationCreateCommandHandler> logger) : IRequestHandler<ObservationCreateCommand, ServiceResult<int>>
{
    public async Task<ServiceResult<int>> Handle(ObservationCreateCommand request, CancellationToken cancellationToken)
    {
        if(request == null)
        {
            logger.LogError("ObservationCreateCommand request is null");
            return ServiceResult<int>.Error("ObservationCreateCommand request is null");
        }
        // 1. Create or Get Authority
        var authority = await GetOrCreateAuthority(request, cancellationToken);
        if (!authority.IsSuccess)
            return ServiceResult<int>.Error(authority.ErrorList);

        // 2. Create or Get Family
        var family = await GetOrCreateFamily(request, cancellationToken);
        if (!family.IsSuccess)
            return ServiceResult<int>.Error(family.ErrorList);

        // 3. Create or Get Genus
        var genus = await GetOrCreateGenus(request, family.Data.Id, cancellationToken);
        if (!genus.IsSuccess)
            return ServiceResult<int>.Error(genus.ErrorList);

        // 4. Create or Get SpeciesType
        var speciesType = await GetOrCreateSpeciesType(request, cancellationToken);
        if (!speciesType.IsSuccess)
            return ServiceResult<int>.Error(speciesType.ErrorList);

        // 5. Create or Get Species
        var species = await GetOrCreateSpecies(request, authority.Data.Id, genus.Data.Id, speciesType.Data.Id, cancellationToken);
        if (!species.IsSuccess)
            return ServiceResult<int>.Error(species.ErrorList);

        // 6. Create or Get Province
        var province = await GetOrCreateProvince(request, cancellationToken);
        if (!province.IsSuccess)
            return ServiceResult<int>.Error(province.ErrorList);

        // 7. Create or Get Location
        var location = await GetOrCreateLocation(request, province.Data.Id, cancellationToken);
        if (!location.IsSuccess)
            return ServiceResult<int>.Error(location.ErrorList);

        // 8. Create or Get Observer
        var observer = await GetOrCreateObserver(request, cancellationToken);
        if (!observer.IsSuccess)
            return ServiceResult<int>.Error(observer.ErrorList);

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
        return ServiceResult<int>.Success(observation.Id);

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
            logger.LogInformation("Authority created that has id : {0}",authority.Id);
        }
        return authority != null ?
            ServiceResult<Authority>.Success(authority) :
            ServiceResult<Authority>.Error("Failed to create authority");
    }
    private async Task<ServiceResult<Family>> GetOrCreateFamily(ObservationCreateCommand request, CancellationToken cancellationToken)
    {

            var family = await  familyRepository.GetByNameAsync(request.FamilyName, cancellationToken);
            if (family == null && !string.IsNullOrEmpty(request.FamilyName))
            {
                family = new Family { Name = request.FamilyName };
                await  familyRepository.AddAsync(family, cancellationToken);
                await  unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return family != null ?
                ServiceResult<Family>.Success(family) :
                ServiceResult<Family>.Error("Failed to create family");

    }

    private async Task<ServiceResult<Genus>> GetOrCreateGenus(ObservationCreateCommand request, int familyId, CancellationToken cancellationToken)
    {

            var genus = await  genusRepository.GetByNameAndFamilyIdAsync(request.GenusName, familyId, cancellationToken);
            if (genus == null && !string.IsNullOrEmpty(request.GenusName))
            {
                genus = new Genus
                {
                    Name = request.GenusName,
                    FamilyId = familyId
                };
                await  genusRepository.AddAsync(genus, cancellationToken);
                await  unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return genus != null ?
                ServiceResult<Genus>.Success(genus) :
                ServiceResult<Genus>.Error("Failed to create genus");
    }

    private async Task<ServiceResult<SpeciesType>> GetOrCreateSpeciesType(ObservationCreateCommand request, CancellationToken cancellationToken)
    {

            var speciesType = await  speciesTypeRepository.GetByNameAsync(request.SpeciesTypeName, cancellationToken);
            if (speciesType == null && !string.IsNullOrEmpty(request.SpeciesTypeName))
            {
                speciesType = new SpeciesType
                {
                    Name = request.SpeciesTypeName,
                    Description = request.SpeciesTypeDescription
                };
                await  speciesTypeRepository.AddAsync(speciesType, cancellationToken);
                await  unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return speciesType != null ?
                ServiceResult<SpeciesType>.Success(speciesType) :
                ServiceResult<SpeciesType>.Error("Failed to create species type");

    }

    private async Task<ServiceResult<Species>> GetOrCreateSpecies(ObservationCreateCommand request, int authorityId, int genusId, int speciesTypeId, CancellationToken cancellationToken)
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
        if (species == null)
            return ServiceResult<Species>.Error("Species could not be found or created");
        return ServiceResult<Species>.Success(species);
    }

    private async Task<ServiceResult<Province>> GetOrCreateProvince(ObservationCreateCommand request, CancellationToken cancellationToken)
    {

            var province = await  provinceRepository.GetByNameAsync(request.ProvinceName, cancellationToken);
            if (province == null && !string.IsNullOrEmpty(request.ProvinceName))
            {
                province = new Province
                {
                    Name = request.ProvinceName,
                    ProvinceCode = request.ProvinceCode
                };
                await  provinceRepository.AddAsync(province, cancellationToken);
                await  unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return province != null ?
                ServiceResult<Province>.Success(province) :
                ServiceResult<Province>.Error("Failed to create province");

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
    private async Task<Location> FindExistingLocation(ObservationCreateCommand request)
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
    private async Task<ServiceResult<Observer>> GetOrCreateObserver(ObservationCreateCommand request, CancellationToken cancellationToken)
    {

            var observer = await  observerRepository.GetByNameAndSurnameAsync(request.ObserverName, request.Surname, cancellationToken);
            if (observer == null && !string.IsNullOrEmpty(request.ObserverName))
            {
                observer = new Observer
                {
                    Name = request.ObserverName,
                    Surname = request.Surname,
                    FullName = request.ObserverFullName
                };
                await  observerRepository.AddAsync(observer, cancellationToken);
                await  unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return observer != null ?
                ServiceResult<Observer>.Success(observer) :
                ServiceResult<Observer>.Error("Failed to create observer");
        
    }
}
