using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.EmailCommands;
public class ResendEmailConfirmMailCommand : IRequest<ServiceResult>
{
    public string Email { get; set; }
}
