using BioWings.Application.Features.Queries.RoleQueries;
using BioWings.Application.Features.Results.RoleResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.RoleHandlers.Read;
public class RoleGetQueryHandler(IRoleRepository roleRepository, ILogger<RoleGetQueryHandler> logger) : IRequestHandler<RoleGetQuery, ServiceResult<List<RoleGetQueryResult>>>
{
    public async Task<ServiceResult<List<RoleGetQueryResult>>> Handle(RoleGetQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.GetAllAsNoTracking().Include(x => x.UserRoles).ToListAsync(cancellationToken);
        var result = roles.Select(x => new RoleGetQueryResult
        {
            Id=x.Id,
            Name=x.Name,
            UserCount=x.UserRoles.Count
        }).ToList();
        if (result.Count==0)
        {
            logger.LogInformation("No roles found");
            return ServiceResult<List<RoleGetQueryResult>>.Success(new List<RoleGetQueryResult>(), HttpStatusCode.NoContent);
        }
        logger.LogInformation("Fetching all roles");
        return ServiceResult<List<RoleGetQueryResult>>.Success(result);

    }
}
