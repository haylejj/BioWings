using BioWings.Application.DTOs.CountryDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.CountryCommands;
public class CountryCreateRangeCommand : IRequest<ServiceResult>
{
    public List<CountryCreateDto> Countries { get; set; }
}
