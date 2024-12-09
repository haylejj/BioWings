using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.LocationCommands;

public class LocationRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}