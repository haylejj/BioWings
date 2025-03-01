using BioWings.Application.Features.Commands.RoleCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.RoleHandlers.Write;
public class RoleCreateRangeCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, ILogger<RoleCreateRangeCommandHandler> logger) : IRequestHandler<RoleCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(RoleCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (!request.Roles.Any())
        {
            logger.LogError("RoleCreateRangeCommand request is null");
            return ServiceResult.Error("RoleCreateRangeCommand request cannot be null");
        }
        var roles = request.Roles.Select(x => new Role
        {
            Name = x.Name
        }).ToList();
        await roleRepository.AddRangeAsync(roles, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation($"Roles created: {string.Join(", ", roles.Select(x => x.Name))}");
        return ServiceResult.SuccessAsCreated("api/Roles/Range");
    }
}

