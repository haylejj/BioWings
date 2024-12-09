using BioWings.Application.DTOs.AuthorityDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.AuthorityCommands;
public class AuthorityCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
    public int Year { get; set; }
}
public class AuthorityCreateRangeCommand : IRequest<ServiceResult>
{
    public List<AuthorityCreateDto> AuthorityCreateDtos { get; set; }
}
public class AuthorityUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Year { get; set; }
}
public class AuthorityRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
