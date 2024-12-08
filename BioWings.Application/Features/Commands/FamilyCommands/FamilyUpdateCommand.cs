using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.FamilyCommands;

public class FamilyUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
