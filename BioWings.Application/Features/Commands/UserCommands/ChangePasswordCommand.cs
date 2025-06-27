using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.UserCommands;

public class ChangePasswordCommand : IRequest<ServiceResult>
{
    public int UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
} 