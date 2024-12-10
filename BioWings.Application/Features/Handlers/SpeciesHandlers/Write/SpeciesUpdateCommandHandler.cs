using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesUpdateCommandHandler(ISpeciesRepository speciesRepository, IUnitOfWork unitOfWork, ILogger<SpeciesUpdateCommandHandler> logger) : IRequestHandler<SpeciesUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesUpdateCommand request, CancellationToken cancellationToken)
    {
        var species = await speciesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (species == null)
        {
            logger.LogError("Species is not found that has id:{0}", request.Id);
            return ServiceResult.Error($"Species is not found that has id: {request.Id}", System.Net.HttpStatusCode.NotFound);
        }
        species.AuthorityId = request.AuthorityId;
        species.GenusId = request.GenusId;
        species.SpeciesTypeId = request.SpeciesTypeId;
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
        if (request.NewFiles.Count > 0)
        {
            //TODO: Add new files to media
        }
        if (!string.IsNullOrEmpty(request.DeletedFilesIds))
        {
            var deletedFilesIds = request.DeletedFilesIds.Split(',');
            //TODO: Remove files from media
        }
        speciesRepository.Update(species);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Species is updated that has id:{0}", request.Id);
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
