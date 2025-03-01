using BioWings.Application.Features.Results.LoginResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.LoginCommands;
public class LoginCommand : IRequest<ServiceResult<LoginResult>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
