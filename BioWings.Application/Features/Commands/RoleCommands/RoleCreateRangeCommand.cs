using BioWings.Application.DTOs.RoleDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.RoleCommands;
public class RoleCreateRangeCommand : IRequest<ServiceResult>
{
    public List<RoleCreateDto> Roles { get; set; } = new List<RoleCreateDto>();
}
