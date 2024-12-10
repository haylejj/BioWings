using BioWings.Application.DTOs.SpeciesDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesCommands;

public class SpeciesCreateRangeCommand : IRequest<ServiceResult>
{
    public List<SpeciesCreateDto> SpeciesCreateDtos { get; set; }
}
