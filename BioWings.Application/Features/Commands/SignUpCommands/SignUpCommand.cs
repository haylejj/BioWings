using BioWings.Application.Features.Results.SignUpResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SignUpCommands;
public class SignUpCommand : IRequest<ServiceResult<SignUpResult>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public int CountryId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
