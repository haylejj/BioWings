using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.AuthorityCommands;

public class AuthorityUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Year { get; set; }
}
