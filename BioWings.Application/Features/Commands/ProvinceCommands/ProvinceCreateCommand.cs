using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ProvinceCommands;
public class ProvinceCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
    public int ProvinceCode { get; set; }
}
