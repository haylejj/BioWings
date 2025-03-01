using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.RoleCommands;

public class RoleUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
