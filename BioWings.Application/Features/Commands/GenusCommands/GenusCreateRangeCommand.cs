using BioWings.Application.DTOs.GenusDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.GenusCommands;
public class GenusCreateRangeCommand : IRequest<ServiceResult>
{
    public List<GenusCreateDto> Genera { get; set; }
}
