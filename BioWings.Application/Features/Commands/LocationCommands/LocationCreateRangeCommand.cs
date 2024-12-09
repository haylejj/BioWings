using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.LocationCommands;

public class LocationCreateRangeCommand : IRequest<ServiceResult>
{
    public List<LocationCreateCommand> LocationCreateCommands { get; set; }
}
