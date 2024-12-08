using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesTypeCommands;
public class SpeciesTypeRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
