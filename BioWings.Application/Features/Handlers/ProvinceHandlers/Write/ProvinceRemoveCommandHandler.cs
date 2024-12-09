using BioWings.Application.Features.Commands.ProvinceCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.ProvinceHandlers.Write;

public class ProvinceRemoveCommandHandler(IProvinceRepository provinceRepository, ILogger<ProvinceRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<ProvinceRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ProvinceRemoveCommand request, CancellationToken cancellationToken)
    {
        var province = await provinceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (province == null)
        {
            logger.LogError("Province with id {Id} not found", request.Id);
            return ServiceResult.Error("Province not found", HttpStatusCode.NotFound);
        }
        provinceRepository.Remove(province);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Province with id {Id} has been removed", request.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
