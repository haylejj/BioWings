using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesTypeCommands;
public class SpeciesTypeCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
    public string Description { get; set; }
}
