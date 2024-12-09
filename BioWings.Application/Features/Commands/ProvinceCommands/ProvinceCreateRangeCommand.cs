using BioWings.Application.DTOs.ProvinceDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ProvinceCommands;

public class ProvinceCreateRangeCommand : IRequest<ServiceResult>
{
    public List<ProvinceCreateDto> ProvinceCreateDtos { get; set; }
}
