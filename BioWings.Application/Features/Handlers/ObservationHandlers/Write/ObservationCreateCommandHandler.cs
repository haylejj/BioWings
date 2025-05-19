using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Application.Validators.Observation;
using BioWings.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationCreateCommandHandler(IObservationRepository observationRepository, ISpeciesRepository speciesRepository, ISubspeciesRepository subspeciesRepository, ILocationRepository locationRepository, IProvinceRepository provinceRepository, IObserverRepository observerRepository, IAuthorityRepository authorityRepository, IFamilyRepository familyRepository, IGenusRepository genusRepository, IUnitOfWork unitOfWork, ILogger<ObservationCreateCommandHandler> logger,IValidator<ObservationCreateCommand> validator) : IRequestHandler<ObservationCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("ObservationCreateCommand request is null");
            return ServiceResult.Error("ObservationCreateCommand request is null");
        }
        //Fluent Validation
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            logger.LogWarning("Observation validation failed: {Errors}", errors);
            return ServiceResult.Error(errors);
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
        var genus = await GetOrCreateGenus(request, family.Data?.Id, cancellationToken);
        if (!genus.IsSuccess)
            return ServiceResult.Error(genus.ErrorList);

        // 5. Create or Get Species
        var species = await GetOrCreateSpecies(request, genus.Data?.Id, authority.Data?.Id, cancellationToken);
        if (!species.IsSuccess)
            return ServiceResult.Error(species.ErrorList);

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
            LocationId = location.Data?.Id,
            ObserverId = observer.Data?.Id,
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
        if (string.IsNullOrEmpty(request.AuthorityName)) return ServiceResult<Authority>.Success(null);

        var authority = await authorityRepository.GetByNameAndYearAsync(request.AuthorityName, request.Year, cancellationToken);
        if (authority == null)
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
        return ServiceResult<Authority>.Success(authority);
    }

    private async Task<ServiceResult<Family>> GetOrCreateFamily(ObservationCreateCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FamilyName)) return ServiceResult<Family>.Success(null);

        var family = await familyRepository.GetByNameAsync(request.FamilyName, cancellationToken);
        if (family == null)
        {
            family = new Family { Name = request.FamilyName };
            await familyRepository.AddAsync(family, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return ServiceResult<Family>.Success(family);
    }

    private async Task<ServiceResult<Genus>> GetOrCreateGenus(ObservationCreateCommand request, int? familyId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.GenusName)) return ServiceResult<Genus>.Success(null);

        var genus = await genusRepository.GetByNameAsync(request.GenusName, cancellationToken);
        if (genus == null)
        {
            genus = new Genus
            {
                Name = request.GenusName,
                FamilyId = familyId
            };
            await genusRepository.AddAsync(genus, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return ServiceResult<Genus>.Success(genus);
    }

    private async Task<ServiceResult<Species>> GetOrCreateSpecies(ObservationCreateCommand request, int? genusId, int? authorityId, CancellationToken cancellationToken)
    {
        var species = await speciesRepository.GetByName_Authority_GenusAsync(request.Name, request.AuthorityName, request.GenusName, request.Year, cancellationToken);
        if (species == null)
        {
            species = new Species
            {
                GenusId = genusId,
                AuthorityId = authorityId,
                ScientificName = request.ScientificName,
                Name = request.Name,
                EUName = request.EUName,
                FullName = request.FullName,
                HesselbarthName = request.HesselbarthName,
                TurkishName = request.TurkishName,
                EnglishName = request.EnglishName,
                TurkishNamesTrakel = request.TurkishNamesTrakel,
                Trakel = request.Trakel,
                KocakName = request.KocakName
            };
            await speciesRepository.AddAsync(species, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return ServiceResult<Species>.Success(species);
    }

    private async Task<ServiceResult<Location>> GetOrCreateLocation(ObservationCreateCommand request, int? provinceId, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetByCoordinatesAsync(Math.Round(request.Latitude, 6),Math.Round(request.Longitude, 6),request.SquareRef,request.CoordinatePrecisionLevel,cancellationToken);

        if (location == null)
        {
            location = new Location
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
            await locationRepository.AddAsync(location, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ServiceResult<Location>.Success(location);
    }

    private async Task<ServiceResult<Observer>> GetOrCreateObserver(ObservationCreateCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ObserverFullName) && string.IsNullOrEmpty(request.ObserverName))
            return ServiceResult<Observer>.Success(null);

        var fullName = request.ObserverFullName ?? request.ObserverName;
        var observer = await observerRepository.GetByFullNameAsync(fullName, cancellationToken);
        if (observer == null)
        {
            observer = new Observer
            {
                Name = request.ObserverName,
                Surname = request.Surname,
                FullName = fullName
            };
            await observerRepository.AddAsync(observer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return ServiceResult<Observer>.Success(observer);
    }
}
