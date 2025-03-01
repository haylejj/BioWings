using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.EmailCommands;
public class ResetPasswordLinkCommand : IRequest<ServiceResult>
{
    public string Email { get; set; }
}
