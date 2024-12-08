using BioWings.Application.DTOs.SubspeciesDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SubspeciesCommands;

public class SubspeciesCreateRangeCommand : IRequest<ServiceResult>
{
    public List<SubspeciesCreateDto> Subspecies { get; set; }
}
