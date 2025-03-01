using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.UserCommands;
public class UserCreateByAdminCommand : IRequest<ServiceResult>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int CountryId { get; set; }
}
