using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.EmailCommands;
public class ChangePasswordCommand : IRequest<ServiceResult>
{
    public string Token { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

}
