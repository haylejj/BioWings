using BioWings.Application.Features.Commands.RoleCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.RoleHandlers.Write;
public class RoleUpdateCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, ILogger<RoleUpdateCommandHandler> logger) : IRequestHandler<RoleUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(RoleUpdateCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
        {
            logger.LogError($"Role with id {request.Id} not found");
            return ServiceResult.Error($"Role not found that has id: {request.Id}", HttpStatusCode.NotFound);
        }
        if (request.Name == null)
        {
            logger.LogError("RoleUpdateCommand request is null");
            return ServiceResult.Error("Role name cannot be null");
        }
        if (await roleRepository.IsExistAsync(x => x.Name == request.Name && x.Id != request.Id, cancellationToken))
        {
            logger.LogError($"Role with name {request.Name} already exists");
            return ServiceResult.Error($"Role with name {request.Name} already exists", HttpStatusCode.Conflict);
        }
        role.Name = request.Name;
        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Role updated: {role.Name} (ID: {role.Id})");
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
