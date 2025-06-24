using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesCreateCommanHandler(ISpeciesRepository speciesRepository, IValidator<SpeciesCreateCommand> validator, IAuthorityRepository authorityRepository, IUnitOfWork unitOfWork, ILogger<SpeciesCreateCommanHandler> logger) : IRequestHandler<SpeciesCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesCreateCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

            logger.LogWarning("Species creation validation failed: {Errors}", string.Join(", ", errorMessages));

            return ServiceResult.Error(errorMessages);
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

        var species = new Species
        {
            AuthorityId = authority?.Id,
            EnglishName = request.EnglishName,
            ScientificName = request.ScientificName,
            Name = request.Name,
            FullName = request.FullName,
            EUName = request.EUName,
            TurkishName = request.TurkishName,
            TurkishNamesTrakel = request.TurkishNamesTrakel,
            Trakel = request.Trakel,
            GenusId = request.GenusId,
            HesselbarthName = request.HesselbarthName,
            KocakName = request.KocakName
        };

        await speciesRepository.AddAsync(species, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Species created successfully with ID: {Id}", species.Id);
        return ServiceResult.SuccessAsCreated($"api/Species/{species.Id}");
    }
}
