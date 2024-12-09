using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ProvinceCommands;

public class ProvinceUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProvinceCode { get; set; }
}
