using BioWings.Application.Features.Commands.RoleCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.RoleHandlers.Write;
public class RoleRemoveCommandHandler(IRoleRepository roleRepository, ILogger<RoleRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<RoleRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(RoleRemoveCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
        {
            logger.LogError($"Role with id {request.Id} not found");
            return ServiceResult.Error($"Role not found that has id: {request.Id}", HttpStatusCode.NotFound);
        }
        roleRepository.Remove(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Role removed: {role.Name} (ID: {role.Id})");
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
