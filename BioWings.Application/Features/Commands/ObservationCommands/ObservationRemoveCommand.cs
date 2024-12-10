using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ObservationCommands;
public class ObservationRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
