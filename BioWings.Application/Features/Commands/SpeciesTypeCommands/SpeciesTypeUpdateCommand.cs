using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesTypeCommands;
public class SpeciesTypeUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
