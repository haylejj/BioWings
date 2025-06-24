using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesUpdateCommandHandler(ISpeciesRepository speciesRepository, IGenusRepository genusRepository, IValidator<SpeciesUpdateCommand> validator, IAuthorityRepository authorityRepository, IUnitOfWork unitOfWork, ILogger<SpeciesUpdateCommandHandler> logger) : IRequestHandler<SpeciesUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesUpdateCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            logger.LogWarning("Species update validation failed: {Errors}", string.Join(", ", errorMessages));
            return ServiceResult.Error(errorMessages);
        }

        var species = await speciesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (species == null)
        {
            logger.LogError("Species is not found that has id:{0}", request.Id);
            return ServiceResult.Error($"Species is not found that has id: {request.Id}", System.Net.HttpStatusCode.NotFound);
        }

        Authority? authority = null;
        if (!string.IsNullOrEmpty(request.AuthorityName) && request.AuthorityYear.HasValue)
        {
            authority = await authorityRepository.GetByNameAndYearAsync(
                request.AuthorityName,
                request.AuthorityYear.Value,
                cancellationToken);

            if (authority == null)
            {
                authority = new Authority
                {
                    Name = request.AuthorityName,
                    Year = request.AuthorityYear.Value
                };
                await authorityRepository.AddAsync(authority, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        //if (request.FamilyId.HasValue)
        //{
        //    var genus = await genusRepository.GetByIdAsync(request.GenusId.GetValueOrDefault(0), cancellationToken);
        //    if (genus == null || genus.FamilyId != request.FamilyId)
        //    {
        //        return ServiceResult.Error("Selected Genus does not belong to the selected Family.");
        //    }
        //}

        species.AuthorityId = authority?.Id;
        species.GenusId = request.GenusId;
        species.ScientificName = request.ScientificName;
        species.Name = request.Name;
        species.EUName = request.EUName;
        species.FullName = request.FullName;
        species.TurkishName = request.TurkishName;
        species.EnglishName = request.EnglishName;
        species.TurkishNamesTrakel = request.TurkishNamesTrakel;
        species.Trakel = request.Trakel;
        species.KocakName = request.KocakName;
        species.HesselbarthName = request.HesselbarthName;

        speciesRepository.Update(species);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Species is updated that has id:{0}", request.Id);
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
