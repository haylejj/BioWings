using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesCommands;

public class SpeciesRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
