using BioWings.Application.DTOs.ObservationDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ObservationCommands;
public class ObservationCreateRangeCommand : IRequest<ServiceResult>
{
    public List<ObservationCreateDto> ObservationCreateDtos { get; set; }
}
