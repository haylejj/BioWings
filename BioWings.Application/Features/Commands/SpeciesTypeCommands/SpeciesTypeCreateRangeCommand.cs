using BioWings.Application.DTOs.SpeciesTypeDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesTypeCommands;
public class SpeciesTypeCreateRangeCommand : IRequest<ServiceResult>
{
    public List<SpeciesTypeCreateDto> SpeciesTypes { get; set; }
}

