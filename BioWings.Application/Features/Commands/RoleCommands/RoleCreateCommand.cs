using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.RoleCommands;
public class RoleCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
}
