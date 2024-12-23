using BioWings.Application.Features.Commands.ProvinceCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Write;
public class ProvinceCreateCommandHandler(IProvinceRepository provinceRepository, ILogger<ProvinceCreateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<ProvinceCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ProvinceCreateCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogError("ProvinceCreateCommand request is null");
            return ServiceResult.Error("ProvinceCreateCommand request is null", HttpStatusCode.BadRequest);
        }
        var province = new Province
        {
            Name = request.Name,
            ProvinceCode = request.ProvinceCode
        };
        await provinceRepository.AddAsync(province, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Province {Name} with code {ProvinceCode} has been created", province.Name, province.ProvinceCode);
        return ServiceResult.SuccessAsCreated("api/Provinces/"+province.Id);
    }
}
