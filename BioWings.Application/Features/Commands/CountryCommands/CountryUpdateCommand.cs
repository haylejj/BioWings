using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.CountryCommands;
public class CountryUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
