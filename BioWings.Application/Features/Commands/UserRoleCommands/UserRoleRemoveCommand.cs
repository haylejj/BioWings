using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.UserRoleCommands;

public class UserRoleRemoveCommand : IRequest<ServiceResult>
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}
