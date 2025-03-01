using BioWings.Application.Features.Queries.RoleQueries;
using BioWings.Application.Features.Results.RoleResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.RoleHandlers.Read;
public class RoleGetByIdQueryHandler(IRoleRepository roleRepository, ILogger<RoleGetByIdQueryHandler> logger) : IRequestHandler<RoleGetByIdQuery, ServiceResult<RoleGetByIdQueryResult>>
{
    public async Task<ServiceResult<RoleGetByIdQueryResult>> Handle(RoleGetByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (role == null)
        {
            logger.LogError($"Role with id {request.Id} not found");
            return ServiceResult<RoleGetByIdQueryResult>.Error($"Role not found that has id: {request.Id}", HttpStatusCode.NotFound);
        }

        logger.LogInformation($"Successfully retrieved role: {role.Name} (ID: {request.Id})");

        var result = new RoleGetByIdQueryResult
        {
            Id = role.Id,
            Name = role.Name
        };

        return ServiceResult<RoleGetByIdQueryResult>.Success(result);
    }
}
