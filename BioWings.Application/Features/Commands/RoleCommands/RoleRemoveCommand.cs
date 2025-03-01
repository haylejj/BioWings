using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.RoleCommands;

public class RoleRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
