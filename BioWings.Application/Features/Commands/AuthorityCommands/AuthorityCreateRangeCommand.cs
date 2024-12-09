using BioWings.Application.DTOs.AuthorityDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.AuthorityCommands;

public class AuthorityCreateRangeCommand : IRequest<ServiceResult>
{
    public List<AuthorityCreateDto> AuthorityCreateDtos { get; set; }
}
