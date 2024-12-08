using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.FamilyCommands;
public class FamilyCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
}
