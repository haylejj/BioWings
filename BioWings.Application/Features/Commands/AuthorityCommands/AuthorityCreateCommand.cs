using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.AuthorityCommands;
public class AuthorityCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
    public int Year { get; set; }
}
