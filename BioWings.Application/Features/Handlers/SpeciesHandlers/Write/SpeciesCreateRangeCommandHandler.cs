using BioWings.Application.Features.Commands.SpeciesCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.SpeciesHandlers.Write;
public class SpeciesCreateRangeCommandHandler(ISpeciesRepository speciesRepository, IUnitOfWork unitOfWork, ILogger<SpeciesCreateCommanHandler> logger) : IRequestHandler<SpeciesCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(SpeciesCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request == null || !request.SpeciesCreateDtos.Any())
        {
            logger.LogError("SpeciesCreateRangeCommand request is null or empty");
            return ServiceResult.Error("SpeciesCreateRangeCommand request is null or empty");
        }
        var result = request.SpeciesCreateDtos.Select(x => new Species
        {
            AuthorityId=x.AuthorityId,
            EnglishName=x.EnglishName,
            ScientificName=x.ScientificName,
            Name=x.Name,
            FullName=x.FullName,
            EUName=x.EUName,
            TurkishName=x.TurkishName,
            TurkishNamesTrakel=x.TurkishNamesTrakel,
            Trakel=x.Trakel,
            GenusId=x.GenusId,
            KocakName=x.KocakName,
            HesselbarthName=x.HesselbarthName
        });
        foreach (var item in request.SpeciesCreateDtos)
        {
            if (item.FormFiles != null || item.FormFiles.Count > 0)
            {
                // buraya file upload kısmı gelecek.
            }
        }
        await speciesRepository.AddRangeAsync(result, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Species are created");
        return ServiceResult.SuccessAsCreated("api/Species/Range");
    }
}
