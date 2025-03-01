using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.UserCommands;
public class UserUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public int CountryId { get; set; }
}
