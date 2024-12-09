using BioWings.Application.Features.Commands.ProvinceCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Write;

public class ProvinceCreateRangeCommandHandler(IProvinceRepository provinceRepository, ILogger<ProvinceCreateRangeCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<ProvinceCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ProvinceCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("ProvinceCreateRangeCommand request is null");
            return ServiceResult.Error("ProvinceCreateRangeCommand request is null", HttpStatusCode.BadRequest);
        }
        var provinces = request.ProvinceCreateDtos.Select(p => new Province
        {
            Name = p.Name,
            ProvinceCode = p.ProvinceCode
        });
        await provinceRepository.AddRangeAsync(provinces, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Provinces have been created");
        return ServiceResult.SuccessAsCreated("api/Provinces/Range");
    }
}
