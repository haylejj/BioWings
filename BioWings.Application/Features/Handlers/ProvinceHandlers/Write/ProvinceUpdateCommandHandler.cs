using BioWings.Application.Features.Commands.ProvinceCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Write;

public class ProvinceUpdateCommandHandler(IProvinceRepository provinceRepository, ILogger<ProvinceUpdateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<ProvinceUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ProvinceUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogError("ProvinceUpdateCommand request is null");
            return ServiceResult.Error("ProvinceUpdateCommand request is null", HttpStatusCode.BadRequest);
        }
        var province = await provinceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (province == null)
        {
            logger.LogError("Province with id {Id} not found", request.Id);
            return ServiceResult.Error("Province not found", HttpStatusCode.NotFound);
        }
        province.Name = request.Name;
        province.ProvinceCode = request.ProvinceCode;
        provinceRepository.Update(province);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Province with id {Id} has been updated", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
