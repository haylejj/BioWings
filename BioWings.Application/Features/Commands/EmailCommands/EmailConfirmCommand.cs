using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.EmailCommands;
public class EmailConfirmCommand : IRequest<ServiceResult>
{
    public string Email { get; set; }
    public string Token { get; set; }
}
