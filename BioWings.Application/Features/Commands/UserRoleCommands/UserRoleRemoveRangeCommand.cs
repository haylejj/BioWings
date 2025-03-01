using BioWings.Application.DTOs.UserRoleDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.UserRoleCommands;

public class UserRoleRemoveRangeCommand : IRequest<ServiceResult>
{
    public List<UserRoleCommonDto> UserRoles { get; set; } = new();
}
