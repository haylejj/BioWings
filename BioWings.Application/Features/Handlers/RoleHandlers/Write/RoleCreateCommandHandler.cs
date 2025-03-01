using BioWings.Application.Features.Commands.RoleCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.RoleHandlers.Write;
public class RoleCreateCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, ILogger<RoleCreateCommandHandler> logger) : IRequestHandler<RoleCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
    {
        if (request.Name == null)
        {
            logger.LogError("RoleCreateComamnd request is null");
            return ServiceResult.Error("Role name cannot be null");
        }
        if (await roleRepository.IsExistAsync(x => x.Name == request.Name))
        {
            logger.LogError($"Role with name {request.Name} already exists");
            return ServiceResult.Error($"Role with name {request.Name} already exists", HttpStatusCode.Conflict);
        }
        var role = new Role
        {
            Name = request.Name
        };
        await roleRepository.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Role created: {role.Name} (ID: {role.Id})");
        return ServiceResult.SuccessAsCreated($"api/Roles/{role.Id}");
    }
}
