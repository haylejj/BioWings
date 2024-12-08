using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SubspeciesCommands;

public class SubspeciesRemoveCommand(int id): IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
