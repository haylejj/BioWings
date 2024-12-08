using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SubspeciesCommands;
public class SubspeciesCreateCommand:IRequest<ServiceResult>
{
    public string Name { get; set; }
    public int SpeciesId { get; set; }
}
