using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.GenusCommands;
public class GenusRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
