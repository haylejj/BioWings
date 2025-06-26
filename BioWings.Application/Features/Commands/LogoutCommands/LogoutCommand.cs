using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.LogoutCommands;

public class LogoutCommand : IRequest<ServiceResult>
{
    public int? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
} 