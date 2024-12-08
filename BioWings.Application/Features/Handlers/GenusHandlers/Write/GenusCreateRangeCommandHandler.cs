using BioWings.Application.Features.Commands.GenusCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioWings.Application.Features.Handlers.GenusHandlers.Write;
internal class GenusCreateRangeCommandHandler(IGenusRepository genusRepository, ILogger<GenusCreateRangeCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<GenusCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(GenusCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if(request == null || !request.Genera.Any())
        {
            logger.LogWarning("GenusCreateRangeCommand is null");
            return ServiceResult.Error("GenusCreateRangeCommand is null");
        }
        var genera = request.Genera.Select(g => new Genus
        {
            Name = g.Name,
            FamilyId = g.FamilyId
        });
        await genusRepository.AddRangeAsync(genera);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Genera created successfully");
        return ServiceResult.SuccessAsCreated("/api/Genera/Range");
    }
}
