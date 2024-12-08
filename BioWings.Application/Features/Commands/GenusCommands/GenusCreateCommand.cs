using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.GenusCommands;
public class GenusCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
    public int FamilyId { get; set; }
}
