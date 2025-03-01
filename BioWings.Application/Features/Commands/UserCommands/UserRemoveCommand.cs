using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.UserCommands;
public class UserRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
