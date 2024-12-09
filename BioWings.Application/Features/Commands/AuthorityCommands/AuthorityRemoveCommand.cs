using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.AuthorityCommands;

public class AuthorityRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
