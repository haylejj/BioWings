using BioWings.Application.DTOs.UserRoleDtos;
using BioWings.Application.Features.Queries.UserRoleQueries;
using BioWings.Application.Features.Results.UserRoleResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.UserRoleHandlers.Read;
public class UserRoleGetByUserIdQueryHandler(IUserRoleRepository userRoleRepository, IRoleRepository roleRepository, IUserRepository userRepository, ILogger<UserRoleGetByUserIdQueryHandler> logger) : IRequestHandler<UserRoleGetByUserIdQuery, ServiceResult<UserRoleGetByUserIdResult>>
{
    public async Task<ServiceResult<UserRoleGetByUserIdResult>> Handle(UserRoleGetByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (!await userRepository.IsExistAsync(x => x.Id==request.Id))
        {
            logger.LogWarning("User with id {UserId} does not exist", request.Id);
            return ServiceResult<UserRoleGetByUserIdResult>.Error($"User with id {request.Id} does not exist.", System.Net.HttpStatusCode.NotFound);
        }
        var userRoles = await userRoleRepository.GetUserRolesByUserIdAsync(request.Id, cancellationToken);
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        var UserRoleGetByUserIdDto = new List<UserRoleGetByUserIdDto>();
        if (roles != null || roles.Any())
        {
            foreach (var role in roles)
            {
                var isSelected = userRoles?.Any(ur => ur.RoleId == role.Id) ?? false;

                UserRoleGetByUserIdDto.Add(new UserRoleGetByUserIdDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = isSelected
                });
            }
        }
        if (!userRoles.Any())
        {
            logger.LogWarning("User with id {UserId} has no roles", request.Id);
            return ServiceResult<UserRoleGetByUserIdResult>.Success(new UserRoleGetByUserIdResult { UserRoles = UserRoleGetByUserIdDto });
        }
        logger.LogInformation("User with id {UserId} has roles {Roles}", request.Id, string.Join(", ", userRoles.Select(x => x.RoleName)));
        return ServiceResult<UserRoleGetByUserIdResult>.Success(new UserRoleGetByUserIdResult { UserRoles = UserRoleGetByUserIdDto });
    }
}
