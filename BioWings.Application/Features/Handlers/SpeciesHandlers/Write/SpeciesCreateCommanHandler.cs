using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesCreateCommanHandler(ISpeciesRepository speciesRepository, IUnitOfWork unitOfWork, ILogger<SpeciesCreateCommanHandler> logger) : IRequestHandler<SpeciesCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesCreateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("SpeciesCreateCommand request is null");
            return ServiceResult.Error("SpeciesCreateCommand request is null");
        }
        var species = new Species
        {
            AuthorityId=request.AuthorityId,
            EnglishName=request.EnglishName,
            ScientificName=request.ScientificName,
            Name=request.Name,
            FullName=request.FullName,
            EUName=request.EUName,
            TurkishName=request.TurkishName,
            TurkishNamesTrakel=request.TurkishNamesTrakel,
            Trakel=request.Trakel,
            GenusId=request.GenusId,
            HesselbarthName=request.HesselbarthName,
            KocakName=request.KocakName
        };
        if (request.FormFiles!=null && request.FormFiles.Count>0)
        {
            // buraya file upload kısmı gelecek.
        }
        await speciesRepository.AddAsync(species, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Species is created that has id {id}", species.Id);
        return ServiceResult.SuccessAsCreated("api/Species/"+species.Id);
    }
}
