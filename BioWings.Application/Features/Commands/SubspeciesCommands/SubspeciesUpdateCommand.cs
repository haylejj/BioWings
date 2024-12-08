using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SubspeciesCommands;

public class SubspeciesUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int SpeciesId { get; set; }
}
