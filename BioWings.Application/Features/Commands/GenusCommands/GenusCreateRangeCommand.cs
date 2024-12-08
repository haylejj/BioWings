using BioWings.Application.DTOs.Genus;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.GenusCommands;
public class GenusCreateRangeCommand : IRequest<ServiceResult>
{
    public IEnumerable<GenusCreateDto> Genera { get; set; }
}
